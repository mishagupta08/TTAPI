using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using TTGarmentsApi.Models;
using GoogleMaps.LocationServices;
using System.Globalization;
using System.Device.Location;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.IO.Compression;

namespace TTGarmentsApi.Repository
{
    public class HomeRepository
    {
        private const double DistanceLimit = 2000;
        private const double RegistrationDistanceLimit = 2000;
        TTLimitedEntities entity = new TTLimitedEntities();

        public async Task<Response> ManageRetailer(R_RetailerMaster retailerDetail, string operation)
        {
            Response responseDetail = new Response();
            try
            {
                if (string.IsNullOrEmpty(operation) || retailerDetail == null)
                {
                    responseDetail.ResponseValue = "Please send complete details.";
                }

                if (operation == "Add")
                {
                    var user = entity.R_RetailerMaster.FirstOrDefault(g => g.Mobile == retailerDetail.Mobile);
                    if (user == null)
                    {
                        if (string.IsNullOrEmpty(retailerDetail.ShopLogo))
                        {
                            responseDetail.ResponseValue = "Please send Shop Logo.";
                        }
                        if (string.IsNullOrEmpty(retailerDetail.ShopGpsX) || string.IsNullOrEmpty(retailerDetail.ShopGpsY))
                        {
                            responseDetail.ResponseValue = "Please send ypur Registration Location Gps.";
                        }
                        if (string.IsNullOrEmpty(retailerDetail.AddressGpsX) || string.IsNullOrEmpty(retailerDetail.AddressGpsY))
                        {
                            responseDetail.ResponseValue = "Please send Shop Location Gps.";
                        }
                        else
                        {
                            //var distance = FindDistance(retailerDetail.ShopGpsX, retailerDetail.ShopGpsY, retailerDetail.AddressGpsX, retailerDetail.AddressGpsY);
                            //if (distance > RegistrationDistanceLimit)
                            //{
                            //    responseDetail.ResponseValue = "Registration should be done near your shop.";
                            //}
                            //else
                            {
                                retailerDetail.ID = Guid.NewGuid().ToString().Substring(0, 5);
                                retailerDetail.RegistrationDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                                entity.R_RetailerMaster.Add(retailerDetail);

                                /***Add Points on registration***/

                                var points = new R_PointsLedger();
                                points.Id = Guid.NewGuid().ToString().Substring(0, 5);
                                points.Barcode = "-Registration Bonus-";
                                points.DabitPoints = 50;
                                points.EarnSpentDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                                points.LocationX = retailerDetail.ShopGpsX;
                                points.LocationY = retailerDetail.ShopGpsY;
                                points.RetailerId = retailerDetail.ID;
                                points.FirmName = retailerDetail.FirmName;
                                points.ProductQty = 0;
                                entity.R_PointsLedger.Add(points);

                                /**************End***************/

                                /*****Assign newly added fields*****/

                                var stId = Convert.ToInt32(retailerDetail.StateId);
                                var ctId = Convert.ToInt32(retailerDetail.CityId);

                                var states = await Task.Run(() => entity.R_StateMaster.ToList());
                                var cities = await Task.Run(() => entity.R_CityMaster.ToList());
                                var pointsList = await Task.Run(() => entity.R_PointsLedger.ToList());


                                var state = await Task.Run(() => states.FirstOrDefault(s => s.Id == stId));
                                if (state != null)
                                {
                                    retailerDetail.StateName = state.Name;
                                }
                                var city = await Task.Run(() => cities.FirstOrDefault(s => s.cityID == ctId));
                                if (city != null)
                                {
                                    retailerDetail.CityName = city.cityName;
                                }

                                retailerDetail.Points = 50;
                                retailerDetail.Distance = Convert.ToDecimal(FindDistance(retailerDetail.AddressGpsX, retailerDetail.AddressGpsY, retailerDetail.ShopGpsX, retailerDetail.ShopGpsY));
                                retailerDetail.Distance = retailerDetail.Distance / 1000;

                                /**************End***************/

                                await entity.SaveChangesAsync();
                                responseDetail.Status = true;
                                responseDetail.Points = 50;
                                responseDetail.ResponseValue = retailerDetail.ID;
                            }
                        }
                    }
                    else
                    {
                        responseDetail.ResponseValue = "Mobile No already exist.";
                    }
                }
                else if (operation == "Edit" || operation == "AddNotificationId" || operation == "Delete" || operation == "Approve" || operation == "Reject")
                {
                    var user = entity.R_RetailerMaster.FirstOrDefault(g => g.ID == retailerDetail.ID);
                    if (user == null)
                    {
                        responseDetail.ResponseValue = "Retailer not found.";
                    }
                    else
                    {
                        if (operation == "Approve")
                        {
                            user.IsApproved = true;
                        }
                        else if (operation == "Reject")
                        {
                            user.IsApproved = false;
                        }
                        else if (operation == "AddNotificationId")
                        {
                            if (string.IsNullOrEmpty(retailerDetail.NotificationId))
                            {
                                responseDetail.ResponseValue = "Please send notification id.";
                            }
                            else
                            {
                                user.NotificationId = retailerDetail.NotificationId;
                                responseDetail.Status = true;
                                responseDetail.ResponseValue = "Notification id added successfully.";
                            }
                        }
                        else if (operation == "Edit")
                        {
                            user.Address = retailerDetail.Address;
                            user.CityId = retailerDetail.CityId;
                            user.DistributorId = retailerDetail.DistributorId;
                            user.FirmName = retailerDetail.FirmName;
                            user.IsActive = retailerDetail.IsActive;
                            //user.Mobile = retailerDetail.Mobile;
                            user.Email = retailerDetail.Email;
                            ////if (retailerDetail.RegistrationDate != null)
                            ////{
                            ////    user.RegistrationDate = retailerDetail.RegistrationDate;
                            ////}

                            user.PinCode = retailerDetail.PinCode;
                            user.ShopGpsX = retailerDetail.ShopGpsX;
                            user.ShopGpsY = retailerDetail.ShopGpsY;
                            user.AddressGpsX = retailerDetail.AddressGpsX;
                            user.AddressGpsY = retailerDetail.AddressGpsY;
                            user.StateId = retailerDetail.StateId;
                            user.DistributerCity = retailerDetail.DistributerCity;
                            user.DistributerName = retailerDetail.DistributerName;
                            user.DistributerMobileNo = retailerDetail.DistributerMobileNo;
                            user.IsBlock = retailerDetail.IsBlock;
                            /*****Assign newly added fields*****/

                            var stId = Convert.ToInt32(retailerDetail.StateId);
                            var ctId = Convert.ToInt32(retailerDetail.CityId);

                            var states = await Task.Run(() => entity.R_StateMaster.ToList());
                            var cities = await Task.Run(() => entity.R_CityMaster.ToList());
                            var pointsList = await Task.Run(() => entity.R_PointsLedger.Where(r=>r.RetailerId == retailerDetail.ID).ToList());


                            var state = await Task.Run(() => states.FirstOrDefault(s => s.Id == stId));
                            if (state != null)
                            {
                                user.StateName = state.Name;
                            }
                            var city = await Task.Run(() => cities.FirstOrDefault(s => s.cityID == ctId));
                            if (city != null)
                            {
                                user.CityName = city.cityName;
                            }

                            user.Points = await this.GetCurrentBalacePoints(pointsList, retailerDetail.ID);
                            retailerDetail.Distance = Convert.ToDecimal(FindDistance(retailerDetail.AddressGpsX, retailerDetail.AddressGpsY, retailerDetail.ShopGpsX, retailerDetail.ShopGpsY));
                            user.Distance = retailerDetail.Distance / 1000;

                            /**************End***************/

                            responseDetail.Status = true;
                            responseDetail.ResponseValue = "Retailer edited successfully.";
                        }
                        else
                        {
                            entity.R_RetailerMaster.Remove(user);
                            responseDetail.Status = true;
                            responseDetail.ResponseValue = "Retailer deleted successfully.";
                        }

                        await entity.SaveChangesAsync();
                    }
                }
                else if (operation == "ById")
                {
                    var user = entity.R_RetailerMaster.FirstOrDefault(g => g.ID == retailerDetail.ID);
                    if (user == null)
                    {
                        responseDetail.ResponseValue = "No Records found.";
                    }
                    else
                    {
                        var states = await Task.Run(() => entity.R_StateMaster.ToList());
                        var cities = await Task.Run(() => entity.R_CityMaster.ToList());
                        if (states != null && cities != null)
                        {
                            var state = await Task.Run(() => states.FirstOrDefault(s => Convert.ToString(s.Id) == user.StateId));
                            if (state != null)
                            {
                                user.StateName = state.Name;
                            }

                            var city = await Task.Run(() => cities.FirstOrDefault(s => Convert.ToString(s.cityID) == user.CityId));

                            if (city != null)
                            {
                                user.CityName = city.cityName;
                            }
                        }

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(user);
                    }
                }
                else if (operation == "List")
                {
                    var list = await Task.Run(() => entity.R_RetailerMaster.ToList());
                    if (list == null || list.Count() == 0)
                    {
                        responseDetail.ResponseValue = "No Records found.";
                    }
                    else
                    {
                        //var states = await Task.Run(() => entity.R_StateMaster.ToList());
                        //var cities = await Task.Run(() => entity.R_CityMaster.ToList());
                        //var pointsList = await Task.Run(() => entity.R_PointsLedger.ToList());
                        //  if (states != null && cities != null)
                        {
                            foreach (var data in list)
                            {
                                //var stId = Convert.ToInt32(data.StateId);
                                //var ctId = Convert.ToInt32(data.CityId);

                                //var state = await Task.Run(() => states.FirstOrDefault(s => s.Id == stId));
                                //if (state != null)
                                //{
                                //    data.StateName = state.Name;
                                //}
                                //var city = await Task.Run(() => cities.FirstOrDefault(s => s.cityID == ctId));
                                //if (city != null)
                                //{
                                //    data.CityName = city.cityName;
                                //}

                                //data.Points = await this.GetCurrentBalacePoints(pointsList, data.ID);

                                if (data.RegistrationDate != null)
                                {
                                    data.DateString = data.RegistrationDate.ToString();
                                }

                                //data.Distance = FindDistance(data.AddressGpsX, data.AddressGpsY, data.ShopGpsX, data.ShopGpsY);
                                //data.Distance = data.Distance / 1000;
                            }
                        }
                        responseDetail.Status = true;
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = 2147483644;
                        responseDetail.ResponseValue = serializer.Serialize(list);
                        responseDetail.ResponseValue = Compress(responseDetail.ResponseValue);
                        //responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                    }
                }
            }
            catch (Exception e)
            {
                responseDetail.Status = false;
                responseDetail.ResponseValue = e.Message;
            }

            return responseDetail;
        }

        public async Task<Response> ManagePromotion(R_Promotion promotionDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || promotionDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                promotionDetail.Id = Guid.NewGuid().ToString().Substring(0, 7);
                promotionDetail.IsActive = true;
                promotionDetail.CreatedDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                entity.R_Promotion.Add(promotionDetail);
                await entity.SaveChangesAsync();

                responseDetail.Status = true;
                responseDetail.ResponseValue = "Promotion detail added successfully.";
            }
            else if (operation == "Edit" || operation == "Delete" || operation == "UpdateStatus")
            {
                var detail = entity.R_Promotion.FirstOrDefault(g => g.Id == promotionDetail.Id);
                if (detail == null)
                {
                    responseDetail.ResponseValue = "Promotion detail not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        // detail.IsActive = promotionDetail.IsActive;
                        detail.Heading = promotionDetail.Heading;
                        detail.HeadingText = promotionDetail.HeadingText;
                        detail.ImageCount = promotionDetail.ImageCount;
                        detail.Image = promotionDetail.Image;
                        detail.TotalEntries = promotionDetail.TotalEntries;

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Promotion detail edited successfully.";
                    }
                    else if (operation == "UpdateStatus")
                    {
                        if (detail.IsActive == true)
                        {
                            detail.IsActive = false;
                            responseDetail.ResponseValue = "Activate";
                        }
                        else
                        {
                            detail.IsActive = true;
                            responseDetail.ResponseValue = "DeActivate";
                        }

                        //prod.IsActive = product.IsActive;
                        responseDetail.Status = true;
                    }
                    else
                    {
                        entity.R_Promotion.Remove(detail);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Promotion deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var promotion = entity.R_Promotion.FirstOrDefault(g => g.Id == promotionDetail.Id);
                if (promotion == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(promotion);
                }
            }
            else if (operation == "List" || operation == "FullList")
            {
                var list = await Task.Run(() => entity.R_Promotion.ToList());
                if (operation == "List")
                {
                    list = list.Where(l => l.IsActive == true).ToList();
                }
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageMessages(R_MessageMaster messageDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || messageDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                messageDetail.Id = Guid.NewGuid().ToString().Substring(0, 7);
                messageDetail.IsActive = true;
                messageDetail.MessagePublishDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                entity.R_MessageMaster.Add(messageDetail);
                await entity.SaveChangesAsync();

                responseDetail.Status = true;
                responseDetail.ResponseValue = "Message detail added successfully.";
            }
            else if (operation == "Edit" || operation == "Delete" || operation == "UpdateStatus")
            {
                var detail = entity.R_MessageMaster.FirstOrDefault(g => g.Id == messageDetail.Id);
                if (detail == null)
                {
                    responseDetail.ResponseValue = "Message detail not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        // detail.IsActive = promotionDetail.IsActive;
                        detail.Header = messageDetail.Header;
                        detail.Message = messageDetail.Message;
                        detail.ImageUrl = messageDetail.ImageUrl;
                        detail.PublishFrom = messageDetail.PublishFrom;
                        detail.PublishTo = messageDetail.PublishTo;

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Message detail edited successfully.";
                    }
                    else if (operation == "UpdateStatus")
                    {
                        if (detail.IsActive == true)
                        {
                            detail.IsActive = false;
                            responseDetail.ResponseValue = "Activate";
                        }
                        else
                        {
                            detail.IsActive = true;
                            responseDetail.ResponseValue = "DeActivate";
                        }

                        //prod.IsActive = product.IsActive;
                        responseDetail.Status = true;
                    }
                    else
                    {
                        entity.R_MessageMaster.Remove(detail);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Message deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var msg = entity.R_MessageMaster.FirstOrDefault(g => g.Id == messageDetail.Id);
                if (msg == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    msg.DateString = msg.MessagePublishDate.ToString();
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(msg);
                }
            }
            else if (operation == "LatestActiveNotification")
            {
                var msg = await GetLatestActiveMessage();
                if (msg == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    /*****Need this parsing so that already running code will not affected.*****/
                    var NotificationMsg = new R_NotificationManager();

                    NotificationMsg.Id = msg.Id;
                    NotificationMsg.ImageUrl = msg.ImageUrl;
                    NotificationMsg.Notification = msg.Message;
                    NotificationMsg.Header = msg.Header;
                    NotificationMsg.DateString = msg.DateString;

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(NotificationMsg);
                }

            }
            else if (operation == "List" || operation == "FullList")
            {
                var list = await Task.Run(() => entity.R_MessageMaster.ToList());
                if (operation == "List")
                {
                    list = list.Where(l => l.IsActive == true).ToList();
                }
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    foreach (var data in list)
                    {
                        data.DateString = data.MessagePublishDate.ToString();
                        data.PublishFromDate = data.PublishFrom.ToString();
                        data.PublishToDate = data.PublishTo.ToString();
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        private async Task<R_MessageMaster> GetLatestActiveMessage()
        {
            var msg = await Task.Run(() => entity.R_MessageMaster.Where(p => p.IsActive == true).OrderBy(g => g.MessagePublishDate).FirstOrDefault());
            if (msg == null)
            {
                return null;
                //responseDetail.ResponseValue = "No Records found.";
            }
            else
            {
                var today = DateTime.Today;
                if (msg.PublishTo < today)
                {
                    msg.IsActive = false;
                    await entity.SaveChangesAsync();
                    return null;
                    //responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    msg.DateString = msg.MessagePublishDate.ToString();
                    return msg;
                }
            }

            return msg;
        }

        //private async Task GetLatestActiveMessage(Response responseDetail)
        //{
        //    var msg = entity.R_MessageMaster.Where(p => p.IsActive == true).OrderBy(g => g.MessagePublishDate).FirstOrDefault();
        //    if (msg == null)
        //    {
        //        responseDetail.ResponseValue = "No Records found.";
        //    }
        //    else
        //    {
        //        var today = DateTime.Today;
        //        if (msg.PublishTo < today)
        //        {
        //            msg.IsActive = false;
        //            await entity.SaveChangesAsync();
        //            responseDetail.ResponseValue = "No Records found.";
        //        }
        //        else
        //        {
        //            msg.DateString = msg.MessagePublishDate.ToString();
        //            responseDetail.Status = true;
        //            responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(msg);
        //        }
        //    }
        //}

        public async Task<Response> ManageRetailerWithFilter(Filters filter)
        {
            Response responseDetail = new Response();
            try
            {
                if (filter == null)
                {
                    responseDetail.ResponseValue = "Please send filter parameters.";
                }
                if (string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    responseDetail.ResponseValue = "Please select both from date or To date";
                }
                else
                {
                    var retailerList = new List<R_RetailerMaster>();
                    if (!string.IsNullOrEmpty(filter.SelectedFilterName))
                    {
                        filter.FilterValue = filter.FilterValue.ToLower();

                        if (filter.SelectedFilterName == "Id")
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(r => r.ID.ToLower().Contains(filter.FilterValue)).ToList());
                        }
                        if (filter.SelectedFilterName == "Date")
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(r => (!string.IsNullOrEmpty(r.DateString) && r.DateString.ToLower().Contains(filter.FilterValue))).ToList());
                        }
                        if (filter.SelectedFilterName == "Firm Name")
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(r => (!string.IsNullOrEmpty(r.FirmName) && r.FirmName.ToLower().Contains(filter.FilterValue))).ToList());
                        }
                        if (filter.SelectedFilterName == "Mobile Number")
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(r => (!string.IsNullOrEmpty(r.Mobile) && r.Mobile.Contains(filter.FilterValue))).ToList());
                        }
                        if (filter.SelectedFilterName == "City")
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(r => (!string.IsNullOrEmpty(r.CityName) && r.CityName.ToLower().Contains(filter.FilterValue))).ToList());
                        }
                        if (filter.SelectedFilterName == "State")
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(r => (!string.IsNullOrEmpty(r.StateName) && r.StateName.ToLower().Contains(filter.FilterValue))).ToList());
                        }
                    }
                    if (!string.IsNullOrEmpty(filter.FromDate))
                    {
                        //String format = "dd-MM-yyyy";
                        DateTime d1 = DateTime.Parse(filter.FromDate, CultureInfo.CurrentCulture);

                        if (string.IsNullOrEmpty(filter.FilterValue))
                        {
                            retailerList = await Task.Run(() => this.entity.R_RetailerMaster.Where(p => p.RegistrationDate >= d1).ToList());
                        }
                        else
                        {
                            retailerList = await Task.Run(() => retailerList.Where(p => p.RegistrationDate >= d1).ToList());
                        }
                    }

                    if (!string.IsNullOrEmpty(filter.ToDate))
                    {
                        filter.ToDate = filter.ToDate.Trim();

                        String format = "dd-MM-yyyy";
                        DateTime d2 = DateTime.Parse(filter.ToDate, null);

                        //if (pointsLedgerList == null)
                        //{
                        //    pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(p => p.EarnSpentDate <= d2).ToList());
                        //}
                        //else
                        {
                            retailerList = await Task.Run(() => retailerList.Where(p => p.RegistrationDate <= d2).ToList());
                        }
                    }

                    foreach (var data in retailerList)
                    {
                        data.DateString = data.RegistrationDate.ToString();
                    }

                    responseDetail.Status = true;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = 2147483644;
                    responseDetail.ResponseValue = serializer.Serialize(retailerList);

                }
            }
            catch (Exception e)
            {
                responseDetail.Status = false;
            }

            return responseDetail;
        }

        public async Task<Response> ManagePointsWithFilter(Filters filter)
        {
            Response responseDetail = new Response();
            try
            {
                if (filter == null)
                {
                    responseDetail.ResponseValue = "Please send filter parameters.";
                }
                if (string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    responseDetail.ResponseValue = "Please select both from date or To date";
                }
                else
                {
                    var pointsLedgerList = new List<R_PointsLedger>();
                    var stri =  EncodeBarcode(filter.FilterValue, ConfigurationManager.AppSettings["Salt"]);
                    if (!string.IsNullOrEmpty(filter.FilterValue))
                    {
                        filter.FilterValue = filter.FilterValue.ToLower();

                        if (filter.SelectedFilterName == "Retailer Id")
                        {
                            pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(r => r.RetailerId.ToLower().Contains(filter.FilterValue)).ToList());
                        }
                        if (filter.SelectedFilterName == "Firm Name")
                        {
                            pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(r => (!string.IsNullOrEmpty(r.FirmName) && r.FirmName.ToLower().Contains(filter.FilterValue))).ToList());
                        }
                        if (filter.SelectedFilterName == "Barcode")
                        {
                            pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(r => (!string.IsNullOrEmpty(r.Barcode) && r.Barcode.ToLower().Contains(filter.FilterValue))).ToList());
                        }
                        if (filter.SelectedFilterName == "Date")
                        {
                            pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(r => (!string.IsNullOrEmpty(r.EarnSpentDateString) && r.Barcode.Contains(filter.FilterValue))).ToList());
                        }
                    }
                    if (!string.IsNullOrEmpty(filter.FromDate))
                    {
                        //String format = "dd-MM-yyyy";
                        DateTime d1 = DateTime.Parse(filter.FromDate, CultureInfo.CurrentCulture);

                        if (string.IsNullOrEmpty(filter.FilterValue))
                        {
                            pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(p => p.EarnSpentDate >= d1).ToList());
                        }
                        else
                        {
                            pointsLedgerList = await Task.Run(() => pointsLedgerList.Where(p => p.EarnSpentDate >= d1).ToList());
                        }
                    }

                    if (!string.IsNullOrEmpty(filter.ToDate))
                    {
                        filter.ToDate = filter.ToDate.Trim();

                        String format = "dd-MM-yyyy";
                        DateTime d2 = DateTime.Parse(filter.ToDate, null);

                        //if (pointsLedgerList == null)
                        //{
                        //    pointsLedgerList = await Task.Run(() => this.entity.R_PointsLedger.Where(p => p.EarnSpentDate <= d2).ToList());
                        //}
                        //else
                        {
                            pointsLedgerList = await Task.Run(() => pointsLedgerList.Where(p => p.EarnSpentDate <= d2).ToList());
                        }
                    }

                    foreach (var data in pointsLedgerList)
                    {
                        data.EarnSpentDateString = data.EarnSpentDate.ToString();
                    }

                    responseDetail.Status = true;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = 2147483644;
                    responseDetail.ResponseValue = serializer.Serialize(pointsLedgerList);

                }
            }
            catch (Exception e)
            {
                responseDetail.Status = false;
            }

            return responseDetail;
        }

        public async Task<Response> ManagePoints(R_PointsLedger pointsLedger, string operation)
        {
            Response responseDetail = new Response();
            try
            {
                if (string.IsNullOrEmpty(operation) || pointsLedger == null)
                {
                    responseDetail.ResponseValue = "Please send complete details.";
                }

                if (operation == "List")
                {
                    var list = await Task.Run(() => entity.R_PointsLedger.ToList());
                    if (list == null || list.Count() == 0)
                    {
                        responseDetail.ResponseValue = "No Records found.";
                    }
                    else
                    {
                        foreach (var data in list)
                        {
                            data.EarnSpentDateString = data.EarnSpentDate.ToString();
                        }

                        responseDetail.Status = true;
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = 2147483644;
                        responseDetail.ResponseValue = serializer.Serialize(list);

                    }
                }
                else if (operation == "BalancePoints")
                {
                    var pointsList = await Task.Run(() => entity.R_PointsLedger);
                    responseDetail.Points = await this.GetCurrentBalacePoints(pointsList, pointsLedger.RetailerId);
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = string.Empty;
                }
                else if (operation == "ById" || operation == "BalancePointById")
                {
                    var list = await Task.Run(() => entity.R_PointsLedger.Where(p => p.RetailerId == pointsLedger.RetailerId));
                    if (list == null || list.Count() == 0)
                    {
                        responseDetail.ResponseValue = "No Records found.";
                    }
                    else
                    {
                        var retailerList = await Task.Run(() => entity.R_RetailerMaster.ToList());
                        var productList = await Task.Run(() => entity.R_ProductMaster.ToList());

                        foreach (var data in list)
                        {
                            var retailer = await Task.Run(() => retailerList.FirstOrDefault(d => d.ID == data.RetailerId));
                            if (retailer != null)
                            {
                                data.FirmName = retailer.FirmName;
                            }

                            var product = await Task.Run(() => productList.FirstOrDefault(d => d.Id == data.ProductId));
                            if (product != null)
                            {
                                data.ProductName = product.Name;
                            }

                            data.EarnSpentDateString = data.EarnSpentDate.ToString();
                        }

                        list = list.OrderBy(p => p.EarnSpentDate);
                        ///**Group list work**/
                        if (operation == "BalancePointById")
                        {
                            var groupedCustomerList = list.GroupBy(u => u.EarnSpentDate).Select(grp => grp.ToList()).ToList();
                            var listData = new List<R_PointsLedger>();

                            foreach (var data in groupedCustomerList)
                            {
                                var date = data.FirstOrDefault().EarnSpentDate;
                                var dt = new R_PointsLedger();
                                dt.EarnSpentDate = date;
                                dt.EarnSpentDateString = date.ToString();
                                dt.DabitPoints = data.Sum(p => p.DabitPoints);
                                dt.CreditPoints = data.Sum(p => p.CreditPoints);
                                dt.BalancePoint = ((dt.DabitPoints ?? 0) - (dt.CreditPoints ?? 0));
                                listData.Add(dt);
                            }
                        }
                        ///**Group list work**/

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                    }
                }
                else if (operation == "BalacePointSummary")
                {
                    var list = await Task.Run(() => entity.R_PointsLedger.Where(p => p.RetailerId == pointsLedger.RetailerId));
                    if (list == null || list.Count() == 0)
                    {
                        responseDetail.ResponseValue = "No Records found.";
                    }
                    else
                    {
                        var retailerList = await Task.Run(() => entity.R_RetailerMaster.ToList());
                        var productList = await Task.Run(() => entity.R_ProductMaster.ToList());

                        foreach (var data in list)
                        {
                            var retailer = await Task.Run(() => retailerList.FirstOrDefault(d => d.ID == data.RetailerId));
                            if (retailer != null)
                            {
                                data.FirmName = retailer.FirmName;
                            }

                            var product = await Task.Run(() => productList.FirstOrDefault(d => d.Id == data.ProductId));
                            if (product != null)
                            {
                                data.ProductName = product.Name;
                            }

                            data.EarnSpentDateString = data.EarnSpentDate.ToString();
                        }

                        /**Group list work**/

                        var groupedCustomerList = list.GroupBy(u => u.EarnSpentDate).Select(grp => grp.ToList()).ToList();
                        var listData = new List<R_PointsLedger>();

                        foreach (var data in groupedCustomerList)
                        {
                            var date = data.FirstOrDefault().EarnSpentDate;
                            listData.Add(new R_PointsLedger
                            {
                                EarnSpentDate = date,
                                EarnSpentDateString = date.ToString(),
                                DabitPoints = data.Sum(p => p.DabitPoints),
                                CreditPoints = data.Sum(p => p.CreditPoints),
                            });

                        }

                        /**Group list work**/

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(listData);
                    }
                }
                else if (operation.Contains("PointsBy"))
                {
                    var pointsList = await Task.Run(() => entity.R_PointsLedger.Where(d => d.RetailerId == pointsLedger.RetailerId));
                    if (pointsList == null)
                    {
                        responseDetail.ResponseValue = "No Records found.";
                    }
                    else
                    {
                        if (operation.Contains("EarnPointsByMonth"))
                        {
                            pointsList = pointsList.Where(p => p.EarnSpentDate.Value.Month == pointsLedger.Month && p.CreditPoints == null);
                        }
                        else if (operation.Contains("EarnPointsByYear"))
                        {
                            pointsList = pointsList.Where(p => p.EarnSpentDate.Value.Year == pointsLedger.Year && p.CreditPoints == null);
                        }
                        else if (operation.Contains("SpentPointsByMonth"))
                        {
                            pointsList = pointsList.Where(p => p.EarnSpentDate.Value.Month == pointsLedger.Month && p.DabitPoints == null);
                        }
                        else if (operation.Contains("SpentPointsByYear"))
                        {
                            pointsList = pointsList.Where(p => p.EarnSpentDate.Value.Year == pointsLedger.Year && p.DabitPoints == null);
                        }

                        if (pointsList == null || pointsList.Count() == 0)
                        {
                            responseDetail.ResponseValue = "No Records Found.";
                        }
                        else
                        {
                            var productList = await Task.Run(() => entity.R_ProductMaster.ToList());
                            foreach (var data in pointsList)
                            {
                                var product = await Task.Run(() => productList.FirstOrDefault(d => d.Id == data.ProductId));
                                if (product != null)
                                {
                                    data.ProductName = product.Name;
                                }

                                data.EarnSpentDateString = data.EarnSpentDate.ToString();
                            }

                            pointsList = pointsList.OrderBy(p => p.EarnSpentDate);

                            responseDetail.Status = true;
                            responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(pointsList);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                responseDetail.Status = false;
                responseDetail.ResponseValue = e.Message;
            }

            return responseDetail;
        }

        public async Task<Response> GetProductWithFilter(Filters filter)
        {
            Response responseDetail = new Response();
            if (filter == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            var list = await Task.Run(() => entity.R_ProductMaster.ToList());
            list = list.Where(l => l.IsActive == true).ToList();
            if (list == null || list.Count() == 0)
            {
                responseDetail.ResponseValue = "No Records found.";
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.TotalRecords = list.Count();

                list = list.OrderBy(p => p.Points).ToList();

                if (filter.FromPoint != 0)
                {
                    list = list.Where(p => p.Points >= filter.FromPoint).ToList();
                }

                if (filter.ToPoint != 0)
                {
                    list = list.Where(p => p.Points <= filter.ToPoint).ToList();
                }

                list = await Task.Run(() => list.OrderBy(p => p.Points).Skip(filter.From - 1).Take(filter.To).ToList());

                list = list.OrderBy(p => p.Points).ToList();

                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
            }

            return responseDetail;
        }

        public async Task<Response> ManageProducts(List<R_ProductMaster> bulkList, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation))
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                if (bulkList != null)
                {
                    foreach (var pro in bulkList)
                    {
                        pro.Id = Guid.NewGuid().ToString().Substring(0, 5);
                        pro.IsActive = true;
                        entity.R_ProductMaster.Add(pro);
                    }
                }

                await entity.SaveChangesAsync();
                responseDetail.Status = true;
                responseDetail.ResponseValue = "Product added successfully.";
            }
            else if (operation == "ById")
            {
                var product = bulkList.FirstOrDefault();
                var prod = entity.R_ProductMaster.FirstOrDefault(g => g.Id == product.Id);
                if (prod == null)
                {
                    responseDetail.ResponseValue = "Product not found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(prod);
                }
            }
            else if (operation == "Edit" || operation == "Delete" || operation == "UpdateStatus")
            {
                var product = bulkList.FirstOrDefault();
                var prod = entity.R_ProductMaster.FirstOrDefault(g => g.Id == product.Id);
                if (prod == null)
                {
                    responseDetail.ResponseValue = "Product not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        prod.Image = product.Image;
                        prod.Name = product.Name;
                        prod.Points = product.Points;
                        prod.ProductCode = product.ProductCode;
                        prod.Size = product.Size;
                        prod.Quantity = product.Quantity;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Product edited successfully.";
                    }
                    else if (operation == "UpdateStatus")
                    {
                        if (prod.IsActive == true)
                        {
                            prod.IsActive = false;
                            responseDetail.ResponseValue = "Activate";
                        }
                        else
                        {
                            prod.IsActive = true;
                            responseDetail.ResponseValue = "DeActivate";
                        }
                        //prod.IsActive = product.IsActive;
                        responseDetail.Status = true;

                    }
                    else
                    {
                        entity.R_ProductMaster.Remove(prod);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Product deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "List" || operation == "FullList")
            {
                var list = await Task.Run(() => entity.R_ProductMaster.ToList());
                if (operation == "List")
                {
                    list = list.Where(l => l.IsActive == true).ToList();
                }

                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageAdminUser(R_AdminMaster adminDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || adminDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                var user = entity.R_AdminMaster.FirstOrDefault(g => g.Email == adminDetail.Email && g.Password == adminDetail.Password);
                if (user == null)
                {
                    adminDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                    entity.R_AdminMaster.Add(adminDetail);
                    await entity.SaveChangesAsync();

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = "Registration done successfully.";
                }
                else
                {
                    responseDetail.ResponseValue = "Email already exist.";
                }
            }
            else if (operation == "Edit" || operation == "Delete")
            {
                var user = entity.R_AdminMaster.FirstOrDefault(g => g.Id == adminDetail.Id);
                if (user == null)
                {
                    responseDetail.ResponseValue = "Retailer not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        user.IsActive = adminDetail.IsActive;
                        user.Name = adminDetail.Name;
                        user.MobileNo = adminDetail.MobileNo;
                        user.Email = adminDetail.Email;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Admin edited successfully.";
                    }
                    else
                    {
                        entity.R_AdminMaster.Remove(user);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Admin deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var user = entity.R_AdminMaster.FirstOrDefault(g => g.Id == adminDetail.Id);
                if (user == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(user);
                }
            }
            else if (operation == "List")
            {
                var list = await Task.Run(() => entity.R_AdminMaster.ToList());
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> StateList()
        {
            Response responseDetail = new Response();

            var states = await Task.Run(() => entity.R_StateMaster.ToList());

            if (states == null || states.Count() == 0)
            {
                responseDetail.ResponseValue = "No State Found";
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(states);
            }

            return responseDetail;
        }

        public async Task<Response> CityList(int stateCode)
        {
            Response responseDetail = new Response();

            var cities = await Task.Run(() => entity.R_CityMaster.Where(c => c.StateId == stateCode));

            if (cities == null || cities.Count() == 0)
            {
                responseDetail.ResponseValue = "No Cities Found";
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(cities);
            }

            return responseDetail;
        }

        public async Task<Response> GetDistributerCityList()
        {
            Response responseDetail = new Response();

            var cities = await Task.Run(() => entity.R_DistributerMaster.Select(c => c.City).Distinct().OrderBy(o => o.Substring(0)));

            if (cities == null || cities.Count() == 0)
            {
                responseDetail.ResponseValue = "No Cities Found";
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(cities);
            }

            return responseDetail;
        }

        public async Task<Response> GetDistributerByCityName(string CityName)
        {
            Response responseDetail = new Response();

            var distributers = await Task.Run(() => entity.R_DistributerMaster.Where(c => c.City == CityName).OrderBy(c => c.Name));

            if (distributers == null || distributers.Count() == 0)
            {
                responseDetail.ResponseValue = "No Distributer Found";
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(distributers);
            }

            return responseDetail;
        }

        public async Task<Response> ListAppDownloadDetail()
        {
            Response responseDetail = new Response();

            var detail = await Task.Run(() => entity.R_AppDownloadDetail.ToList());

            if (detail == null)
            {
                responseDetail.ResponseValue = "No Detail Found";
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(detail);
            }

            return responseDetail;
        }

        public async Task<Response> ManageFetivePoints(R_FestivePointMaster detail, string operation)
        {
            Response responseDetail = new Response();
            try
            {
                if (string.IsNullOrEmpty(operation))
                {
                    responseDetail.ResponseValue = "Please send complete detail";
                }
                else
                {
                    if (operation == "Add")
                    {
                        detail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                        detail.IsActive = true;
                        detail.AddedDate = DateTime.Now;
                        entity.R_FestivePointMaster.Add(detail);

                        await this.entity.SaveChangesAsync();

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Festive Point Detail Added successfully.";
                    }
                    else if (operation == "ById" || operation == "Edit" || operation == "UpdateStatus")
                    {
                        var data = entity.R_FestivePointMaster.FirstOrDefault(g => g.Id == detail.Id);
                        if (data == null)
                        {
                            responseDetail.ResponseValue = "No Records found.";
                        }
                        else
                        {
                            if (operation == "ById")
                            {
                                responseDetail.Status = true;
                                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(data);
                            }
                            else if (operation == "UpdateStatus")
                            {
                                if (data.IsActive == true)
                                {
                                    data.IsActive = false;
                                    responseDetail.ResponseValue = "Activate";
                                }
                                else
                                {
                                    data.IsActive = true;
                                    responseDetail.ResponseValue = "DeActivate";
                                }

                                responseDetail.Status = true;
                                await entity.SaveChangesAsync();
                            }
                            else if (operation == "Edit")
                            {
                                data.FromDate = detail.FromDate;
                                data.ToDate = detail.ToDate;
                                data.FromPoint = detail.FromPoint;
                                data.ToPoint = detail.ToPoint;

                                responseDetail.Status = true;
                                responseDetail.ResponseValue = "Festival Point Edited Successfully.";
                                await entity.SaveChangesAsync();
                            }


                        }
                    }
                    else if (operation == "List")
                    {
                        var list = await Task.Run(() => entity.R_FestivePointMaster.ToList());

                        if (list == null || list.Count() == 0)
                        {
                            responseDetail.ResponseValue = "No Records found.";
                        }
                        else
                        {
                            foreach (var data in list)
                            {
                                data.DateString = data.AddedDate.ToString();
                                data.FromDateString = data.FromDate.ToString();
                                data.ToDateString = data.ToDate.ToString();
                            }

                            responseDetail.Status = true;
                            responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                responseDetail.Status = false;
            }

            return responseDetail;
        }

        public async Task<Response> AddAppDownloadDetail(string deviceId)
        {
            Response responseDetail = new Response();

            var detail = await Task.Run(() => entity.R_AppDownloadDetail.FirstOrDefault(c => c.DeviceId == deviceId));

            if (detail == null)
            {
                var downloadDetail = new R_AppDownloadDetail();
                downloadDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                downloadDetail.DeviceId = deviceId;
                downloadDetail.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                entity.R_AppDownloadDetail.Add(downloadDetail);
                await entity.SaveChangesAsync();
            }
            else
            {
                responseDetail.Status = true;
                responseDetail.ResponseValue = "Detail already exist.";
            }

            return responseDetail;
        }

        public async Task<Response> LoginRetailer(string username, string password)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            var user = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(g => g.Mobile == username && g.Password == password && g.IsActive == true));

            if (user == null)
            {
                responseDetail.ResponseValue = "Invalid username or password.";
            }
            else
            {
                var pointsList = await Task.Run(() => entity.R_PointsLedger);
                user.Points = await this.GetCurrentBalacePoints(pointsList, user.ID);
                await this.entity.SaveChangesAsync();
                responseDetail.Points = user.Points ?? 0;
                responseDetail.Status = true;
                responseDetail.CartProductCount = await CartProductCount(user.ID);
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(user);
            }

            return responseDetail;
        }

        public async Task<Response> LoginAdminUser(string username, string password)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            try
            {
                var user = await Task.Run(() => entity.R_AdminMaster.FirstOrDefault(g => g.Username == username && g.Password == password));

                if (user == null)
                {
                    responseDetail.ResponseValue = "Invalid username or password.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(user);
                }
            }

            catch (Exception ex)
            {
                if ( ex.InnerException != null && ex.InnerException.StackTrace != null)
                {
                    responseDetail.ResponseValue = ex.InnerException.Message;
                }
            }

            return responseDetail;
        }

        public async Task<Response> SaveImage(ProfilePhoto detail)
        {
            var responseDetail = new Response();
            var fileExtension = string.Empty;

            if (!detail.fileName.Contains("."))
            {
                responseDetail.ResponseValue = "Invalid File Name. Please send file name with extension.";
            }
            else
            {
                var fileExt = detail.fileName.Split('.');
                if (fileExt.Length <= 0 || fileExt.Length > 2)
                {
                    responseDetail.ResponseValue = "Invalid File Name.";
                }
                else
                {
                    fileExtension = fileExt[1];
                }
            }
            if (!string.IsNullOrEmpty(fileExtension))
            {
                if (detail == null || string.IsNullOrEmpty(detail.fileBytes) || string.IsNullOrEmpty(detail.fileName) || string.IsNullOrEmpty(detail.fileBytes))
                {
                    responseDetail.ResponseValue = "Please send complete image detail.";
                }
                else
                {
                    try
                    {
                        //var member = new R_RetailerMaster();
                        //if (detail.mediaType != "ProductImage")
                        //{
                        //    member = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(m => m.ID == detail.memberId));
                        //}

                        //if (member == null && detail.mediaType != "ProductImage")
                        //{
                        //    responseDetail.ResponseValue = "Member Not Found";
                        //}
                        //else
                        {
                            //var fileData = data.Split(',');
                            //Byte[] fileBytes = Convert.FromBase64String(fileData[1]);
                            //byte[] fileBytesArray = Encoding.ASCII.GetBytes(detail.fileBytes);

                            string url = UploadImageCode(detail, fileExtension);

                            //if (detail.mediaType == "LOGO")
                            //{
                            //    member.ShopLogo = url;
                            //    await entity.SaveChangesAsync();
                            //}

                            responseDetail.ResponseValue = "Image Uploaded successfully.";
                            responseDetail.Status = true;
                            responseDetail.Url = url;
                        }

                    }
                    catch (Exception e)
                    {
                        responseDetail.ResponseValue = e.Message;
                    }
                }
            }

            return responseDetail;
        }

        private static string UploadImageCode(ProfilePhoto detail, string fileExtension)
        {
            byte[] fileBytesArray = Convert.FromBase64String(detail.fileBytes);

            string myfile = Guid.NewGuid().ToString().Substring(0, 8) + "." + fileExtension;

            var serverPath = HttpContext.Current.Server.MapPath("~/UploadedImage");

            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            var path = Path.Combine(serverPath, myfile);
            System.IO.File.WriteAllBytes(path, fileBytesArray);

            string currentURL = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/UploadedImage/" + myfile;

            var url = Uri.EscapeUriString(currentURL);
            return url;
        }

        public async Task<Response> ManageOrder(OrderRequest detail, string operation)
        {
            Response responseDetail = new Response();
            if (operation == "AddToCart")
            {
                if (detail == null)
                {
                    responseDetail.ResponseValue = "Please send complete details.";
                }

                var retailer = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(p => p.ID == detail.RetailerId));

                if (retailer == null)
                {
                    responseDetail.ResponseValue = "Retailer Not Found.";
                }
                else
                {
                    if (detail.ProductDetail == null || detail.ProductDetail.Count == 0)
                    {
                        responseDetail.ResponseValue = "Please send product detail";
                    }
                    else
                    {
                        foreach (var pid in detail.ProductDetail)
                        {
                            var prod = new R_CartDetail();
                            prod.Id = Guid.NewGuid().ToString().Substring(0, 5);
                            prod.ProductId = pid.ProductId;
                            prod.ProductQuantity = pid.Quantity;
                            prod.RetailerId = detail.RetailerId;
                            entity.R_CartDetail.Add(prod);
                        }

                        await entity.SaveChangesAsync();
                        responseDetail.ResponseValue = "Product Added to cart.";
                        responseDetail.Status = true;
                        responseDetail.CartProductCount = await CartProductCount(detail.RetailerId);
                    }
                }
            }
            else if ((operation == "DeleteCart"))
            {
                if (detail.ProductDetail != null)
                {
                    responseDetail.ResponseValue = "Please send Product Id to delete from cart.";
                }
                var error = false;
                foreach (var pid in detail.ProductDetail)
                {
                    var prod = await Task.Run(() => entity.R_CartDetail.FirstOrDefault(p => p.ProductId == pid.ProductId && p.RetailerId == detail.RetailerId));
                    if (prod == null)
                    {
                        error = true;
                        responseDetail.ResponseValue = "Product Not Found.";
                    }
                    else
                    {
                        entity.R_CartDetail.Remove(prod);
                    }
                }

                await entity.SaveChangesAsync();
                if (!error)
                {
                    responseDetail.ResponseValue = "Product Removed From Cart.";
                    responseDetail.Status = true;
                    responseDetail.CartProductCount = await CartProductCount(detail.RetailerId);
                }
            }
            else if ((operation == "ViewCart"))
            {
                var cartProducts = new List<R_ProductMaster>();
                var cartList = await Task.Run(() => entity.R_CartDetail.Where(p => p.RetailerId == detail.RetailerId));
                if (cartList == null)
                {
                    responseDetail.ResponseValue = "Your cart is empty.";
                }
                else
                {
                    var productList = await Task.Run(() => entity.R_ProductMaster.ToList());
                    foreach (var cart in cartList)
                    {
                        var prod = await Task.Run(() => productList.FirstOrDefault(p => p.Id == cart.ProductId));
                        if (prod != null)
                        {
                            prod.Quantity = cart.ProductQuantity;
                            cartProducts.Add(prod);
                        }
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(cartProducts);
                }
            }
            else if (operation == "Order")
            {
                if (detail == null)
                {
                    responseDetail.ResponseValue = "Please send complete details.";
                }

                var retailer = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(p => p.ID == detail.RetailerId));

                if (retailer == null)
                {
                    responseDetail.ResponseValue = "Retailer Not Found.";
                }
                else
                {
                    if (detail.ProductDetail == null || detail.ProductDetail.Count == 0)
                    {
                        responseDetail.ResponseValue = "Please send product detail";
                    }
                    else
                    {
                        /**Check retailer available points**/
                        var pLedger = new R_PointsLedger { RetailerId = retailer.ID };
                        var pointsResponse = await ManagePoints(pLedger, "BalancePoints");
                        if (pointsResponse == null || !pointsResponse.Status)
                        {
                            responseDetail.ResponseValue = "Not able to get retailer points.";
                        }
                        var productList = new List<R_ProductMaster>();
                        bool isProductUnavailable = false;
                        decimal? totalProductPoints = 0;
                        foreach (var pid in detail.ProductDetail)
                        {
                            var product = await Task.Run(() => entity.R_ProductMaster.FirstOrDefault(p => p.Id == pid.ProductId));
                            if (product == null)
                            {
                                isProductUnavailable = true;
                                responseDetail.ResponseValue = "On or more Product Not Found.";
                            }
                            else if (product.Quantity < pid.Quantity)
                            {
                                isProductUnavailable = true;
                                responseDetail.ResponseValue = "On or more Product is out of stock.";
                            }

                            if (isProductUnavailable)
                            {
                                break;
                            }
                            else
                            {
                                totalProductPoints += product.Points * pid.Quantity;
                                productList.Add(product);
                            }

                        }
                        if (isProductUnavailable)
                        {
                            return responseDetail;
                        }
                        else
                        {
                            var retailerPoints = pointsResponse.Points;

                            if (totalProductPoints > retailerPoints)
                            {
                                responseDetail.ResponseValue = "Sorry! You Insufficient points to Redeem products.";
                            }
                            else
                            {
                                foreach (var product in productList)
                                {
                                    var pid = detail.ProductDetail.FirstOrDefault(p => p.ProductId == product.Id);
                                    pLedger = new R_PointsLedger();
                                    pLedger.Id = Guid.NewGuid().ToString().Substring(0, 5);
                                    pLedger.RetailerId = retailer.ID;
                                    pLedger.EarnSpentDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                                    pLedger.LocationX = detail.LocationX;
                                    pLedger.LocationY = detail.LocationY;
                                    pLedger.ProductId = product.Id;
                                    pLedger.ProductName = product.Name;
                                    pLedger.CreditPoints = product.Points * pid.Quantity;
                                    pLedger.Barcode = "Redeem Product";
                                    pLedger.ProductQty = pid.Quantity;

                                    entity.R_PointsLedger.Add(pLedger);
                                }

                                var orderNo = Guid.NewGuid().ToString().Substring(0, 10);

                                foreach (var product in productList)
                                {
                                    var pid = detail.ProductDetail.FirstOrDefault(p => p.ProductId == product.Id);

                                    var order = new R_OrderMaster();
                                    order.Id = Guid.NewGuid().ToString().Substring(0, 5);
                                    order.ProductId = product.Id;
                                    order.ProductName = product.Name;
                                    order.RetailerName = retailer.FirmName;
                                    order.RetailerId = detail.RetailerId;
                                    order.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                                    order.PointsUsed = product.Points * pid.Quantity;
                                    order.Quantity = pid.Quantity;
                                    order.OrderStatus = Status.Pending.ToString();
                                    order.OrderNo = orderNo;
                                    entity.R_OrderMaster.Add(order);
                                }

                                foreach (var pid in detail.ProductDetail)
                                {
                                    var product = await Task.Run(() => entity.R_ProductMaster.FirstOrDefault(p => p.Id == pid.ProductId));
                                    product.Quantity = product.Quantity - pid.Quantity;

                                    var cartProduct = await Task.Run(() => entity.R_CartDetail.FirstOrDefault(p => p.ProductId == pid.ProductId && p.RetailerId == detail.RetailerId));
                                    if (cartProduct != null)
                                    {
                                        entity.R_CartDetail.Remove(cartProduct);
                                    }
                                }

                                await entity.SaveChangesAsync();
                                var pointsList = await Task.Run(() => entity.R_PointsLedger);
                                responseDetail.Points = await this.GetCurrentBalacePoints(pointsList, detail.RetailerId);
                                retailer.Points = responseDetail.Points;
                                await entity.SaveChangesAsync();

                                responseDetail.Status = true;
                                responseDetail.ResponseValue = "Order has been placed successfully. Product will Deliver at you shop.";
                            }
                        }
                    }
                }
            }
            else if (operation == "List")
            {
                var list = await Task.Run(() => entity.R_OrderMaster.ToList());
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    var retailers = await Task.Run(() => entity.R_RetailerMaster.ToList());
                    var products = await Task.Run(() => entity.R_ProductMaster.ToList());
                    foreach (var order in list)
                    {
                        order.DateString = order.Date.ToString();
                        var product = products.FirstOrDefault(p => p.Id == order.ProductId);
                        if (product != null)
                        {
                            if (string.IsNullOrEmpty(order.ProductName))
                            {
                                order.ProductName = product.Name;
                            }
                        }

                        var retailer = retailers.FirstOrDefault(p => p.ID == order.RetailerId);
                        if (retailer != null)
                        {
                            if (string.IsNullOrEmpty(order.Retailer))
                            {
                                order.Retailer = retailer.FirmName;
                            }
                            order.Address = retailer.Address;
                            order.City = retailer.CityName;
                            order.State = retailer.StateName;
                        }
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }
            else if (operation == "OrderDetailById")
            {
                var list = await Task.Run(() => entity.R_OrderMaster.Where(o => o.RetailerId == detail.RetailerId));
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    var retailers = await Task.Run(() => entity.R_RetailerMaster.ToList());
                    var products = await Task.Run(() => entity.R_ProductMaster.ToList());
                    foreach (var order in list)
                    {
                        var product = products.FirstOrDefault(p => p.Id == order.ProductId);
                        if (product != null)
                        {
                            if (string.IsNullOrEmpty(order.ProductName))
                            {
                                order.ProductName = product.Name;
                            }
                        }

                        var retailer = retailers.FirstOrDefault(p => p.ID == order.RetailerId);
                        if (retailer != null)
                        {
                            if (string.IsNullOrEmpty(order.Retailer))
                            {
                                order.Retailer = retailer.FirmName;
                            }
                            //order.Retailer = retailer.FirmName;
                            order.Address = retailer.Address;
                        }

                        order.DateString = order.Date.ToString();
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> UpdateStatus(R_OrderMaster detail)
        {
            Response responseDetail = new Response();

            var order = await Task.Run(() => entity.R_OrderMaster.FirstOrDefault(c => c.Id == detail.Id));

            if (order == null)
            {
                responseDetail.ResponseValue = "Order detail not found.";
            }
            else
            {
                var previousStatus = order.OrderStatus;
                if (previousStatus == detail.OrderStatus)
                {
                    responseDetail.ResponseValue = "Order is already :" + detail.OrderStatus;
                    responseDetail.Status = true;
                }
                else
                {
                    order.OrderStatus = detail.OrderStatus;
                    if (detail.OrderStatus == Status.Rejected.ToString())
                    {
                        /***Add Points Back on Order Reject***/

                        var points = new R_PointsLedger();
                        points.Id = Guid.NewGuid().ToString().Substring(0, 5);
                        points.Barcode = "-Add Rejected Product Points Back- On Order # " + order.OrderNo;
                        points.DabitPoints = order.PointsUsed;
                        points.EarnSpentDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                        points.RetailerId = order.RetailerId;
                        points.FirmName = order.RetailerName;
                        points.ProductQty = 0;
                        entity.R_PointsLedger.Add(points);


                        /**************End***************/
                    }

                    await entity.SaveChangesAsync();
                    var retailer = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(p => p.ID == order.RetailerId));
               
                    if (detail.OrderStatus == Status.Delivered.ToString())
                    {
                        string message = "Your " + order.OrderNo + " had been delivered. Enjoy your Gift. If you did not receive it properly please inform through at kolkata@ttlimited.co.in within 2 days.";
                        await SendMessage(retailer.Mobile, message);
                    }
                                
                    var pointsList = await Task.Run(() => entity.R_PointsLedger);
                    responseDetail.Points = await this.GetCurrentBalacePoints(pointsList, order.RetailerId);
                    retailer.Points = responseDetail.Points;
                    await entity.SaveChangesAsync();

                    responseDetail.ResponseValue = "Status updated successfully.";
                    responseDetail.Status = true;

                    
                }

                
            }

            return responseDetail;
        }

        public async Task<Response> ScanBarcode(BarcodeDetail barcodeDetail)
        {
            Response responseDetail = new Response();

            if (barcodeDetail == null || string.IsNullOrEmpty(barcodeDetail.RetailerId))
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            // First check if retailer available or not
            var retailer = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(c => c.ID == barcodeDetail.RetailerId));

            if (retailer == null)
            {
                responseDetail.ResponseValue = "No Retailer Found.";
            }
            else
            {
                /******Code to check Scan distance*****/
                /***Un-comment this code for distance*****/
                //var gpsx = retailer.AddressGpsX;
                //var gpsy = retailer.AddressGpsY;
                //if (string.IsNullOrEmpty(gpsx))
                //{
                //    gpsx = retailer.ShopGpsX;
                //}
                //if (string.IsNullOrEmpty(gpsy))
                //{
                //    gpsy = retailer.ShopGpsY;
                //}


                //var distance = FindDistance(gpsx, gpsy, barcodeDetail.LocationX, barcodeDetail.LocationY);
                //if (distance > DistanceLimit)
                //{
                //    responseDetail.ResponseValue = "Scan barcode at your shop, or set your location at high accuracy";
                //}
                //else
                /***Un-comment this code for distance*****/
                {

                    /******Code to check Scan distance*****/

                    // Encrypt barcode and chk with list if barcode available or not.
                    var encrptedBarcode = EncodeBarcode(barcodeDetail.Barcode, ConfigurationManager.AppSettings["Salt"]);
                    var barcode = await Task.Run(() => entity.R_BarcodeMaster.FirstOrDefault(c => c.Barcode == encrptedBarcode && c.IsEncrypted == true));

                    if (barcode == null)
                    {
                        responseDetail.ResponseValue = "Barcode Not Available.";
                    }
                    else
                    {
                        //check if available barcode was used or not
                        if (barcode.IsUsed == true)
                        {
                            responseDetail.ResponseValue = "Barcode Already Used.";
                        }
                        else
                        {
                            // bar code is fresh so dabit points to reatiler.
                            barcode.IsUsed = true;
                            var points = new R_PointsLedger();
                            points.Id = Guid.NewGuid().ToString().Substring(0, 10);
                            points.BarcodeSno = barcode.Sno;
                            points.Barcode = barcodeDetail.Barcode;
                            points.DabitPoints = barcode.Points;
                            points.EarnSpentDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                            points.LocationX = barcodeDetail.LocationX;
                            points.LocationY = barcodeDetail.LocationY;
                            points.RetailerId = barcodeDetail.RetailerId;
                            points.FirmName = retailer.FirmName;
                            points.ProductQty = 0;


                            /*****Check for festive point*****/
                            points.DabitPoints = await CheckPoints(points);

                            entity.R_PointsLedger.Add(points);

                            await entity.SaveChangesAsync();

                            var pointsList = await Task.Run(() => entity.R_PointsLedger.ToList());
                            retailer.Points = await this.GetCurrentBalacePoints(pointsList, retailer.ID);

                            await entity.SaveChangesAsync();
                            responseDetail.Status = true;
                            responseDetail.ResponseValue = points.DabitPoints + " Point Assigned Successfully.";

                        }
                    }
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageMediaCategory(R_MediaCategory categoryDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || categoryDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                categoryDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                categoryDetail.IsActive = true;
                entity.R_MediaCategory.Add(categoryDetail);
                await entity.SaveChangesAsync();
                responseDetail.Status = true;
                responseDetail.ResponseValue = "Category Added Successfully.";
            }
            else if (operation == "Edit" || operation == "Delete" || operation == "UpdateStatus")
            {
                var category = entity.R_MediaCategory.FirstOrDefault(g => g.Id == categoryDetail.Id);
                if (category == null)
                {
                    responseDetail.ResponseValue = "Category not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        category.Category = categoryDetail.Category;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Category edited successfully.";
                    }
                    else if (operation == "UpdateStatus")
                    {
                        if (category.IsActive == true)
                        {
                            category.IsActive = false;
                            responseDetail.ResponseValue = "Activate";
                        }
                        else
                        {
                            category.IsActive = true;
                            responseDetail.ResponseValue = "DeActivate";
                        }

                        //prod.IsActive = product.IsActive;
                        responseDetail.Status = true;
                    }
                    else
                    {
                        entity.R_MediaCategory.Remove(category);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Category deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var category = entity.R_MediaCategory.FirstOrDefault(g => g.Id == categoryDetail.Id);
                if (category == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(category);
                }
            }
            else if (operation == "List" || operation == "FullList")
            {
                var list = await Task.Run(() => entity.R_MediaCategory.ToList());
                if (operation == "List")
                {
                    list = list.Where(l => l.IsActive == true).ToList();
                }
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageUploadedMedia(R_UploadedMedia mediaDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || mediaDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                if (string.IsNullOrEmpty(mediaDetail.Url))
                {
                    responseDetail.ResponseValue = "Please send image Url.";
                }
                else if (string.IsNullOrEmpty(mediaDetail.DisplayName))
                {
                    responseDetail.ResponseValue = "Display Name is empty.";
                }
                else
                {
                    mediaDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                    mediaDetail.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                    mediaDetail.IsActive = true;
                    entity.R_UploadedMedia.Add(mediaDetail);
                    await entity.SaveChangesAsync();
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = "Media Added Successfully.";
                }
            }
            else if (operation == "Edit" || operation == "Delete" || operation == "UpdateStatus")
            {
                var media = entity.R_UploadedMedia.FirstOrDefault(g => g.Id == mediaDetail.Id);
                if (media == null)
                {
                    responseDetail.ResponseValue = "Media not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        media.CategoryId = mediaDetail.CategoryId;
                        media.DisplayName = mediaDetail.DisplayName;
                        media.IsActive = mediaDetail.IsActive;
                        media.Url = mediaDetail.Url;
                        if (mediaDetail.Date != null)
                        {
                            media.Date = mediaDetail.Date;
                        }

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Media edited successfully.";
                    }
                    else if (operation == "UpdateStatus")
                    {
                        if (media.IsActive == true)
                        {
                            media.IsActive = false;
                            responseDetail.ResponseValue = "Activate";
                        }
                        else
                        {
                            media.IsActive = true;
                            responseDetail.ResponseValue = "DeActivate";
                        }

                        //prod.IsActive = product.IsActive;
                        responseDetail.Status = true;
                    }
                    else
                    {
                        entity.R_UploadedMedia.Remove(media);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Media deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var category = entity.R_UploadedMedia.FirstOrDefault(g => g.Id == mediaDetail.Id);
                if (category == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(category);
                }
            }
            else if (operation == "List" || operation == "FullList")
            {
                var list = await Task.Run(() => entity.R_UploadedMedia.ToList());
                if (operation == "List")
                {
                    list = list.Where(l => l.IsActive == true).ToList();
                }
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    var catlist = await Task.Run(() => entity.R_MediaCategory.ToList());

                    foreach (var data in list)
                    {
                        data.CategoryName = catlist.FirstOrDefault(c => c.Id == data.CategoryId).Category;
                        data.DateString = data.Date.ToString();
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageBanner(R_BannerMaster bannerDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || bannerDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                bannerDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                bannerDetail.IsActive = true;
                bannerDetail.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                entity.R_BannerMaster.Add(bannerDetail);
                await entity.SaveChangesAsync();
                responseDetail.Status = true;
                responseDetail.ResponseValue = "Banner Added Successfully.";
            }
            else if (operation == "Edit" || operation == "Delete" || operation == "UpdateStatus")
            {
                var banner = entity.R_BannerMaster.FirstOrDefault(g => g.Id == bannerDetail.Id);
                if (banner == null)
                {
                    responseDetail.ResponseValue = "Banner not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        // banner.IsActive = bannerDetail.IsActive;
                        banner.Url = bannerDetail.Url;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Banner edited successfully.";
                    }
                    else if (operation == "UpdateStatus")
                    {
                        if (banner.IsActive == true)
                        {
                            banner.IsActive = false;
                            responseDetail.ResponseValue = "Activate";
                        }
                        else
                        {
                            banner.IsActive = true;
                            responseDetail.ResponseValue = "DeActivate";
                        }

                        //prod.IsActive = product.IsActive;
                        responseDetail.Status = true;
                    }
                    else
                    {
                        entity.R_BannerMaster.Remove(banner);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Banner deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var category = entity.R_BannerMaster.FirstOrDefault(g => g.Id == bannerDetail.Id);
                if (category == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(category);
                }
            }
            else if (operation == "List" || operation == "FullList")
            {
                var list = await Task.Run(() => entity.R_BannerMaster.ToList());
                if (operation == "List")
                {
                    list = list.Where(l => l.IsActive == true).ToList();
                }
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    foreach (var dt in list)
                    {
                        if (dt.Date != null)
                        {
                            dt.DateString = Convert.ToString(dt.Date);
                        }
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManagePromotionEntry(PromotionEntryDetail entryDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || entryDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                if (string.IsNullOrEmpty(entryDetail.PromoId))
                {
                    responseDetail.ResponseValue = "Please send selected Promotion id.";
                }

                if (string.IsNullOrEmpty(entryDetail.RetailerId))
                {
                    responseDetail.ResponseValue = "Please send Retailer id.";
                }

                var promotionDetail = await Task.Run(() => entity.R_Promotion.FirstOrDefault(p => p.Id == entryDetail.PromoId && p.IsActive == true));

                if (promotionDetail == null)
                {
                    responseDetail.ResponseValue = "Promotion not found or InActive";
                }
                else
                {
                    var promotionEntryList = await Task.Run(() => entity.R_PromotionEntries.Where(p => p.PromotionId == entryDetail.PromoId));
                    var noOfEntriesRegistered = await Task.Run(() => promotionEntryList.Select(o => o.RetailerId).Distinct().Count());
                    var isEmpty = entryDetail.ImageUrls.Any(p => string.IsNullOrEmpty(p));
                    if (noOfEntriesRegistered == promotionDetail.TotalEntries)
                    {
                        responseDetail.ResponseValue = "Registrations are full for this promotion.";
                    }
                    if (isEmpty)
                    {
                        responseDetail.ResponseValue = "One or more image url is empty.";
                    }
                    else
                    {
                        foreach (var url in entryDetail.ImageUrls)
                        {
                            var detail = new R_PromotionEntries();
                            detail.Id = Guid.NewGuid().ToString().Substring(0, 7);
                            detail.RetailerId = entryDetail.RetailerId;
                            detail.PromotionId = entryDetail.PromoId;
                            detail.UpoadedDate = DateTime.Now;
                            detail.ImageUrl = url;
                            entity.R_PromotionEntries.Add(detail);
                        }

                        await entity.SaveChangesAsync();

                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Promotion Entries Done Successfully.";
                    }
                }
            }
            else if (operation == "UpdateRetailerImageStatus")
            {
                var promoDetail = await Task.Run(() => entity.R_Promotion.FirstOrDefault(p => p.Id == entryDetail.PromoId && p.IsActive == true));

                /***Code to update status of a single image***/
                var promotionDetail = await Task.Run(() => entity.R_PromotionEntries.FirstOrDefault(p => p.Id == entryDetail.PromoId));
                if (promotionDetail == null)
                {
                    responseDetail.ResponseValue = "Promotion not found or InActive";
                }
                else
                {
                    promotionDetail.IsValid = entryDetail.IsApproved;
                    await entity.SaveChangesAsync();

                    /**Code to assign points if all images pproved**/
                    await AssignPointsForPromotionApproval(entryDetail.RetailerId, promoDetail.Id, promoDetail.Points, promoDetail.Heading);
                    /**End**/

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = "Image Status Updated Successfully.";
                }
            }
            else if (operation == "ListPromotionWithRetailerStatus")
            {
                //fetch promotion with promo id
                var promotionDetail = await Task.Run(() => entity.R_Promotion.Where(p => p.IsActive == true));
                if (promotionDetail == null)
                {
                    responseDetail.ResponseValue = "Promotion not found or InActive";
                }
                else
                {
                    //Bring list of all promotion entries which have retailer id.
                    var promotionEntryOfRetailer = await Task.Run(() => entity.R_PromotionEntries.Where(p => p.RetailerId == entryDetail.RetailerId).ToList());
                    //Run a loop on promotion to assign a status of particular retaler.
                    foreach (var promotion in promotionDetail)
                    {
                        promotion.Id = promotion.Id.ToLower();
                        var retailerEntry = promotionEntryOfRetailer.Where(p => p.PromotionId.ToLower() == promotion.Id).ToList();
                        if (retailerEntry.Count() == 0)
                        {
                            promotion.RetailerStatus = 1;// If Retailer not applied for this promotion.
                        }
                        else
                        {
                            // If applied then check Admin take any action or not.
                            var res = retailerEntry.All(p => p.IsValid == null);
                            if (res == true)
                            {
                                promotion.RetailerStatus = 2;// If Admin have not take any action on retailer Entry.
                            }
                            else
                            {
                                res = retailerEntry.All(p => p.IsValid == true);
                                if (res == true)
                                {
                                    promotion.RetailerStatus = 3;// If Admin have Approve *all uploaded images.
                                }
                                else
                                {
                                    promotion.RetailerStatus = 4;// If Admin have reject any single image from all uploaded images.
                                }
                            }
                        }
                    }
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(promotionDetail);
                }
            }
            else if (operation == "ListRetailerByPromotion" || operation == "RetailerImages" || operation == "Approve" || operation == "Reject")
            {
                var promotionDetail = await Task.Run(() => entity.R_Promotion.FirstOrDefault(p => p.Id == entryDetail.PromoId && p.IsActive == true));
                if (promotionDetail == null)
                {
                    responseDetail.ResponseValue = "Promotion not found or InActive";
                }
                else
                {
                    var promotionEntryList = await Task.Run(() => entity.R_PromotionEntries.Where(p => p.PromotionId == entryDetail.PromoId));

                    if (operation == "Approve" || operation == "Reject" || operation == "RetailerImages")
                    {
                        var promoEntriesImagesOfRetailer = promotionEntryList.Where(p => p.RetailerId == entryDetail.RetailerId);
                        if (operation == "RetailerImages")
                        {
                            // var promoEntriesImagesOfRetailer = promotionEntryList.Where(p => p.RetailerId == entryDetail.RetailerId);
                            responseDetail.Status = true;
                            responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(promoEntriesImagesOfRetailer);
                        }
                        else
                        {
                            /***Code to update status of a Complete promotion approve reject all images***/
                            //var promoEntriesOfRetailer = promotionEntryList.FirstOrDefault(p => p.RetailerId == entryDetail.RetailerId);
                            if (promoEntriesImagesOfRetailer != null)
                            {
                                foreach (var entry in promoEntriesImagesOfRetailer)
                                {
                                    entry.IsValid = operation == "Approve" ? true : false;
                                }

                                await entity.SaveChangesAsync();

                                /**Code for assign points on approved promotion entry***/
                                await AssignPointsForPromotionApproval(entryDetail.RetailerId, promotionDetail.Id, promotionDetail.Points, promotionDetail.Heading);

                                //  promoEntriesOfRetailer.IsValid = operation == "Approve" ? true : false;
                                // await entity.SaveChangesAsync();

                                responseDetail.Status = true;
                                responseDetail.ResponseValue = "Promotion Entry " + operation;
                            }
                        }
                    }
                    else
                    {

                        var entriesRegistered = await Task.Run(() => promotionEntryList.Select(o => o.RetailerId));

                        var vals = entriesRegistered.Distinct().ToList();

                        var retailerList = await Task.Run(() => entity.R_RetailerMaster.ToList());
                        var list = new List<PromotionEntryDetail>();
                        foreach (var d in vals)
                        {
                            var ret = retailerList.FirstOrDefault(r => r.ID.ToLower() == d.ToLower());
                            var listret = promotionEntryList.Where(p => p.RetailerId == d).ToList();
                            var fCount = listret.Count(ol => (ol.IsValid == true));
                            var UpImagecount = promotionEntryList.Count(p => p.RetailerId == d);
                            list.Add(new PromotionEntryDetail
                            {
                                PromoId = promotionDetail.Id,
                                PromoHeading = promotionDetail.Heading,
                                PromoText = promotionDetail.HeadingText,
                                RetailerId = d,
                                RetailerFirmName = ret == null ? string.Empty : ret.FirmName,
                                UploadImagecount = UpImagecount,
                                IsApproved = (fCount == UpImagecount) ? true : false,
                                ApprovedImagecount = fCount
                            });
                        }

                        responseDetail.Status = true;
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        //serializer.MaxJsonLength = int.MaxValue;
                        responseDetail.ResponseValue = serializer.Serialize(list);
                    }
                }

            }

            return responseDetail;
        }

        public async Task<Response> ManageAppVersion(R_AppVersion versionDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || versionDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                if (versionDetail.Version == 0)
                {
                    responseDetail.ResponseValue = "Please send version no.";
                }
                else
                {
                    versionDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                    versionDetail.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                    entity.R_AppVersion.Add(versionDetail);
                    await entity.SaveChangesAsync();
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = "Version Added Successfully.";
                }
            }
            else if (operation == "Edit" || operation == "Delete")
            {
                var banner = entity.R_AppVersion.FirstOrDefault(g => g.Id == versionDetail.Id);
                if (banner == null)
                {
                    responseDetail.ResponseValue = "App version detail not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        banner.Version = versionDetail.Version;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "App version detail edited successfully.";
                    }
                    else
                    {
                        entity.R_AppVersion.Remove(versionDetail);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "App version detail deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var version = entity.R_AppVersion.FirstOrDefault(g => g.Id == versionDetail.Id);
                if (version == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(version);
                }
            }
            else if (operation == "List")
            {
                var list = await Task.Run(() => entity.R_AppVersion.ToList());
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    foreach (var dt in list)
                    {
                        dt.DateString = Convert.ToString(dt.Date);
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }
            else if (operation == "CheckVersion")
            {
                var version = await Task.Run(() => entity.R_AppVersion.OrderByDescending(r => r.Date).FirstOrDefault());
                if (version == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    if (version.Version == versionDetail.Version)
                    {
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = " You have latest App Version.";
                    }
                    else
                    {
                        responseDetail.ResponseValue = "Latest version " + version.Version + " found, Please Update.";
                    }
                }
            }
            else if (operation == "FetchVersion")
            {
                var version = await Task.Run(() => entity.R_AppVersion.OrderByDescending(r => r.Date).FirstOrDefault());
                if (version == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(version);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageNotification(R_NotificationManager notificationDetail, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || notificationDetail == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                notificationDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                notificationDetail.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                entity.R_NotificationManager.Add(notificationDetail);
                await entity.SaveChangesAsync();
                responseDetail.Status = true;
                responseDetail.ResponseValue = "Notification sending to retailers..";

                //send notification in bakground
                Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotificationToRetailers));
                bgThread.IsBackground = true;
                bgThread.Start((notificationDetail));
            }
            else if (operation == "Edit" || operation == "Delete")
            {
                var notification = entity.R_NotificationManager.FirstOrDefault(g => g.Id == notificationDetail.Id);
                if (notification == null)
                {
                    responseDetail.ResponseValue = "Notification not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        notification.ImageUrl = notificationDetail.ImageUrl;
                        notification.Notification = notificationDetail.Notification;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Notification edited successfully.";
                    }
                    else
                    {
                        entity.R_NotificationManager.Remove(notification);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Notification deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var category = entity.R_NotificationManager.FirstOrDefault(g => g.Id == notificationDetail.Id);
                if (category == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(category);
                }
            }
            else if (operation == "LatestNotification")
            {
                var msg = await GetLatestActiveMessage();
                if (msg == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    /*****Need this parsing so that already running code will not affected.*****/
                    var NotificationMsg = new R_NotificationManager();

                    NotificationMsg.Id = msg.Id;
                    NotificationMsg.ImageUrl = msg.ImageUrl;
                    NotificationMsg.Notification = msg.Message;
                    NotificationMsg.Header = msg.Header;
                    NotificationMsg.DateString = msg.DateString;

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(NotificationMsg);

                    //var category = entity.R_NotificationManager.OrderByDescending(p => p.Date).FirstOrDefault();
                    //if (category == null)
                    //{
                    //    responseDetail.ResponseValue = "No Records found.";
                    //}
                    //else
                    //{
                    //    responseDetail.Status = true;
                    //    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(category);
                    //}
                }
            }
            else if (operation == "NotificationResultById")
            {
                var result = entity.R_NotificationResult.FirstOrDefault(g => g.Id == notificationDetail.Id);
                if (result == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(result);
                }
            }
            else if (operation == "List")
            {
                var list = await Task.Run(() => entity.R_NotificationManager.ToList());
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    var states = await Task.Run(() => entity.R_StateMaster.ToList());
                    var cities = await Task.Run(() => entity.R_CityMaster.ToList());
                    var result = await Task.Run(() => entity.R_NotificationResult.OrderByDescending(p => p.Date).ToList());
                    foreach (var dt in list)
                    {
                        dt.DateString = Convert.ToString(dt.Date);

                        if (dt.StateCode != null && dt.CityCode != null)
                        {
                            var state = await Task.Run(() => states.FirstOrDefault(s => s.Id == dt.StateCode));
                            if (state != null)
                            {
                                dt.StateName = state.Name;
                            }

                            var city = await Task.Run(() => cities.FirstOrDefault(s => s.cityID == dt.CityCode));

                            if (city != null)
                            {
                                dt.CityName = city.cityName;
                            }
                        }
                        var notificationResult = result.FirstOrDefault(r => r.NotificationId == dt.Id);
                        if (notificationResult == null)
                        {
                            dt.ResultDate = "-";
                            dt.NotificationIdCount = 0;
                            dt.FailMessageCount = 0;
                            dt.SuccessMessageCount = 0;
                        }
                        else
                        {
                            dt.NotificationIdCount = notificationResult.NotificationIdCount ?? 0;
                            dt.FailMessageCount = notificationResult.FailMessageCount ?? 0;
                            dt.SuccessMessageCount = notificationResult.SuccessMessageCount ?? 0;
                            dt.ResultDate = notificationResult.Date.ToString();
                            dt.Exception = notificationResult.Exception;
                        }
                    }

                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> ManageNotificationReply(R_NotificationReply notificationReply, string operation)
        {
            Response responseDetail = new Response();
            if (string.IsNullOrEmpty(operation) || notificationReply == null)
            {
                responseDetail.ResponseValue = "Please send complete details.";
            }

            if (operation == "Add")
            {
                notificationReply.Id = Guid.NewGuid().ToString().Substring(0, 5);
                notificationReply.Date = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                entity.R_NotificationReply.Add(notificationReply);
                await entity.SaveChangesAsync();
                responseDetail.Status = true;
                responseDetail.ResponseValue = "Notification Reply Added Successfully.";
            }
            else if (operation == "Edit" || operation == "Delete")
            {
                var notification = entity.R_NotificationReply.FirstOrDefault(g => g.Id == notificationReply.Id);
                if (notification == null)
                {
                    responseDetail.ResponseValue = "Notification not found.";
                }
                else
                {
                    if (operation == "Edit")
                    {
                        notification.ReplyMessage = notificationReply.ReplyMessage;
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Notification Reply edited successfully.";
                    }
                    else
                    {
                        entity.R_NotificationReply.Remove(notification);
                        responseDetail.Status = true;
                        responseDetail.ResponseValue = "Notification Reply deleted successfully.";
                    }

                    await entity.SaveChangesAsync();
                }
            }
            else if (operation == "ById")
            {
                var notification = entity.R_NotificationReply.FirstOrDefault(g => g.Id == notificationReply.Id);
                if (notification == null)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(notification);
                }
            }
            else if (operation == "List")
            {
                var list = await Task.Run(() => entity.R_NotificationReply.ToList());
                if (list == null || list.Count() == 0)
                {
                    responseDetail.ResponseValue = "No Records found.";
                }
                else
                {
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(list);
                }
            }

            return responseDetail;
        }

        public async Task<Response> GetDashboardCounts()
        {
            try
            {
                Response responseDetail = new Response();
                var counts = new DashboardCounts();
                counts.RetailerCount = await Task.Run(() => entity.R_RetailerMaster.Count());
                counts.TotalRedeemPoint = await Task.Run(() => entity.R_PointsLedger.Sum(p => p.CreditPoints) ?? 0);
                counts.TotalEarnPoint = await Task.Run(() => entity.R_PointsLedger.Sum(p => p.DabitPoints) ?? 0);
                counts.SannedBarcodeCount = await Task.Run(() => entity.R_BarcodeMaster.Where(b => b.IsUsed != null).Count());
                counts.SannedBarcode5Count = await Task.Run(() => entity.R_BarcodeMaster.Where(b => b.IsUsed != null && b.Points == 5).Count());
                counts.SannedBarcode10Count = await Task.Run(() => entity.R_BarcodeMaster.Where(b => b.IsUsed != null && b.Points == 10).Count());
                counts.SannedBarcode15Count = await Task.Run(() => entity.R_BarcodeMaster.Where(b => b.IsUsed != null && b.Points == 15).Count());
                var orderList = await Task.Run(() => entity.R_OrderMaster);
                if (orderList != null)
                {
                    counts.PendingOrderCount = orderList.Count(o => o.OrderStatus == Status.Pending.ToString());
                    counts.RejectOrderCount = orderList.Count(o => o.OrderStatus == Status.Rejected.ToString());
                    counts.ConfirmCount = orderList.Count(o => o.OrderStatus == Status.Confirm.ToString());
                    counts.DeliveredOrderCount = orderList.Count(o => o.OrderStatus == Status.Delivered.ToString());
                    counts.OnHoldCount = orderList.Count(o => o.OrderStatus == Status.OnHold.ToString());
                }

                responseDetail.Status = true;
                responseDetail.ResponseValue = new JavaScriptSerializer().Serialize(counts);

                return responseDetail;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Response> ForgotPassword(string mobileNo)
        {
            try
            {
                Response responseDetail = new Response();
                var retailer = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(r => r.Mobile == mobileNo));
                if (retailer == null)
                {
                    responseDetail.ResponseValue = "Retailer not found.";
                }
                else
                {
                    await SendMessage(retailer.Mobile, "Your Forgot Login password for TT Retailer Club app is " + retailer.Password + ".");
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = "Password has been sent to your registered mobile no.";
                }

                return responseDetail;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async void SendNotificationToRetailers(object notificationDetail)
        {
            var successCount = 0;
            var faliedCount = 0;
            var totalIdCount = 0;
            R_NotificationManager notification = (R_NotificationManager)notificationDetail;
            var notificationIdActualCount = 0;
            try
            {
                var retailerList = await Task.Run(() => entity.R_RetailerMaster.ToList());
                var ids = new List<string>();
                if (retailerList != null)
                {
                    if (notification.Boradcast == false)
                    {
                        if (notification.StateCode != 0)
                        {
                            var stateCode = notification.StateCode.ToString();
                            retailerList = retailerList.Where(r => r.StateId == stateCode).ToList();
                        }

                        //if (notification.CityCode != 0)
                        //{
                        //    var cityCode = notification.CityCode.ToString();
                        //    retailerList = retailerList.Where(r => r.CityId == cityCode).ToList();
                        //}
                    }

                    foreach (var retailer in retailerList)
                    {
                        if (!string.IsNullOrEmpty(retailer.NotificationId))
                        {
                            ids.Add(retailer.NotificationId);
                        }
                        else
                        {
                            //**save this as failed - notification Id not found.
                            notificationIdActualCount++;
                        }
                    }
                }

                /******Actual Notification send code start******/

                totalIdCount = ids.Count;
                var bunchCount = totalIdCount / 1000 + ((totalIdCount % 1000 > 0 ? 1 : 0));
                string applicationID = "AAAAhaC4aec:APA91bHIMpcNi5dUGXaXd-hxUpeIr3so2H8UBZhB4Oc5p4SRq11XV4QDF1Em1MihyXD-gLy1eF4Ic9hwLSzdlK2k10DFNGINOl1Lt4zELgbKsHyu3FlgWbc4yoOjGsc8AUa3dQD8cg64GZ4MuwFh3qUjQk2MUhsHTg";

                for (var i = 1; i <= bunchCount; i++)
                {

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";

                    var sendIds = ids.Skip((i - 1) * 1000).Take(1000).ToList();

                    var data1 = new
                    {
                        data = new
                        {
                            message_payload = new
                            {
                                NotificationId = notification.Id,
                                Reply = notification.IsReplyable,
                                Heading = notification.Header,
                                Message = notification.Notification,
                                ImageUrl = notification.ImageUrl
                            }
                        },
                        registration_ids = sendIds
                    };

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data1);

                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                    tRequest.UseDefaultCredentials = true;
                    tRequest.PreAuthenticate = true;
                    tRequest.Credentials = CredentialCache.DefaultCredentials;
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    string str = sResponseFromServer;
                                    var respo = serializer.Deserialize<NotificationResponse>(str);
                                    successCount += respo.success;
                                    faliedCount += respo.failure;
                                    if (i == bunchCount)
                                    {
                                        /**Save response in DB**/

                                        var notiResult = new R_NotificationResult();
                                        notiResult.Id = Guid.NewGuid().ToString().Substring(0, 5);
                                        notiResult.SuccessMessageCount = successCount;
                                        notiResult.FailMessageCount = faliedCount;
                                        notiResult.NotificationIdCount = totalIdCount;
                                        notiResult.NotificationId = notification.Id;
                                        notiResult.Date = DateTime.Now;

                                        entity.R_NotificationResult.Add(notiResult);
                                        await entity.SaveChangesAsync();
                                        /**Save response in DB**/
                                    }
                                    //return str;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                if (string.IsNullOrEmpty(str) && ex.InnerException != null && ex.InnerException.StackTrace != null)
                {
                    str = ex.InnerException.StackTrace.Substring(0, 500);
                }

                if (successCount > 0)
                {
                    str = str + " ** Some Notification Partially send :: Total Id Count # " + totalIdCount + " Success # " + successCount + "Failed # " + faliedCount;
                }

                var notiResult = new R_NotificationResult();
                notiResult.Id = Guid.NewGuid().ToString().Substring(0, 5);
                notiResult.SuccessMessageCount = -1;
                notiResult.FailMessageCount = -1;
                notiResult.NotificationId = notification.Id;
                notiResult.Date = DateTime.Now;
                notiResult.Exception = str;
                entity.R_NotificationResult.Add(notiResult);
                await entity.SaveChangesAsync();
                // return str;
            }
        }

        //public async void SendNotificationToRetailers(object notificationDetail)
        //{
        //    R_NotificationManager notification = (R_NotificationManager)notificationDetail;
        //    var notificationIdCount = 0;
        //    try
        //    {
        //        var retailerList = await Task.Run(() => entity.R_RetailerMaster.ToList());
        //        var ids = new List<string>();
        //        if (retailerList != null)
        //        {
        //            if (notification.Boradcast == false)
        //            {
        //                if (notification.StateCode != 0)
        //                {
        //                    var stateCode = notification.StateCode.ToString();
        //                    retailerList = retailerList.Where(r => r.StateId == stateCode).ToList();
        //                }

        //                if (notification.CityCode != 0)
        //                {
        //                    var cityCode = notification.CityCode.ToString();
        //                    retailerList = retailerList.Where(r => r.CityId == cityCode).ToList();
        //                }
        //            }

        //            foreach (var retailer in retailerList)
        //            {
        //                if (!string.IsNullOrEmpty(retailer.NotificationId))
        //                {
        //                    ids.Add(retailer.NotificationId);
        //                }
        //                else
        //                {
        //                    //**save this as failed - notification Id not found.
        //                    notificationIdCount++;
        //                }
        //            }
        //        }

        //        string applicationID = "AAAAhaC4aec:APA91bHIMpcNi5dUGXaXd-hxUpeIr3so2H8UBZhB4Oc5p4SRq11XV4QDF1Em1MihyXD-gLy1eF4Ic9hwLSzdlK2k10DFNGINOl1Lt4zELgbKsHyu3FlgWbc4yoOjGsc8AUa3dQD8cg64GZ4MuwFh3qUjQk2MUhsHTg";
        //        WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //        tRequest.Method = "post";
        //        tRequest.ContentType = "application/json";

        //        /******Actual Notification send code start******/

        //        var data1 = new
        //        {
        //            data = new
        //            {
        //                message_payload = new
        //                {
        //                    NotificationId = notification.Id,
        //                    Reply = notification.IsReplyable,
        //                    Heading = notification.Header,
        //                    Message = notification.Notification,
        //                }
        //            },
        //            registration_ids = ids
        //        };

        //        var serializer = new JavaScriptSerializer();
        //        var json = serializer.Serialize(data1);

        //        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //        tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

        //        tRequest.UseDefaultCredentials = true;
        //        tRequest.PreAuthenticate = true;
        //        tRequest.Credentials = CredentialCache.DefaultCredentials;
        //        tRequest.ContentLength = byteArray.Length;

        //        using (Stream dataStream = tRequest.GetRequestStream())
        //        {
        //            dataStream.Write(byteArray, 0, byteArray.Length);
        //            using (WebResponse tResponse = tRequest.GetResponse())
        //            {
        //                using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //                {
        //                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                    {
        //                        String sResponseFromServer = tReader.ReadToEnd();
        //                        string str = sResponseFromServer;
        //                        var respo = serializer.Deserialize<NotificationResponse>(str);

        //                        var notiResult = new R_NotificationResult();
        //                        notiResult.Id = Guid.NewGuid().ToString().Substring(0, 5);
        //                        notiResult.SuccessMessageCount = respo.success;
        //                        notiResult.FailMessageCount = respo.failure;
        //                        notiResult.NotificationIdCount = notificationIdCount;
        //                        notiResult.NotificationId = notification.Id;
        //                        notiResult.Date = DateTime.Now;

        //                        entity.R_NotificationResult.Add(notiResult);
        //                        await entity.SaveChangesAsync();
        //                        //return str;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string str = ex.Message;
        //        if (string.IsNullOrEmpty(str) && ex.InnerException != null && ex.InnerException.StackTrace != null)
        //        {
        //            str = ex.InnerException.StackTrace.Substring(0, 500);
        //        }

        //        var notiResult = new R_NotificationResult();
        //        notiResult.Id = Guid.NewGuid().ToString().Substring(0, 5);
        //        notiResult.SuccessMessageCount = -1;
        //        notiResult.FailMessageCount = -1;
        //        notiResult.NotificationId = notification.Id;
        //        notiResult.Date = DateTime.Now;
        //        notiResult.Exception = str;
        //        entity.R_NotificationResult.Add(notiResult);
        //        await entity.SaveChangesAsync();
        //        // return str;
        //    }
        //}


        public async Task<string> SendNotification(string firmName, string password, string deviceId)
        {
            try
            {
                string applicationID = "AAAAhaC4aec:APA91bHIMpcNi5dUGXaXd-hxUpeIr3so2H8UBZhB4Oc5p4SRq11XV4QDF1Em1MihyXD-gLy1eF4Ic9hwLSzdlK2k10DFNGINOl1Lt4zELgbKsHyu3FlgWbc4yoOjGsc8AUa3dQD8cg64GZ4MuwFh3qUjQk2MUhsHTg";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    message_payload = new
                    {
                        SenderName = "TTGarments",
                        SenderID = "0",
                        Reply = "N",
                        Heading = "Password",
                        Message = "Hi, " + firmName + " Your password is : " + password,
                        MessageType = "T",
                        FilePath = "",
                        Validity = "-1",
                        SenderIcon = "",
                        MsgID = "20170322175156239",
                        RefID = "",
                        Points = "",
                        SponseredBy = "",
                        SponserIcon = ""
                    },
                    registration_ids = new string[] { deviceId }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.UseDefaultCredentials = true;
                tRequest.PreAuthenticate = true;
                tRequest.Credentials = CredentialCache.DefaultCredentials;
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                                return str;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return str;
            }
        }

        //public async Task<Response> EncryptBarcodes()
        //{
        //    try
        //    {
        //        Response responseDetail = new Response();

        //        //var unEncryptedBarcodes = await Task.Run(() => entity.R_BarcodeMaster.Where(b => !(b.IsEncrypted == true)));

        //        //var count = unEncryptedBarcodes.Count();
        //        var count = 3000000;
        //        //if (unEncryptedBarcodes == null || count == 0)
        //        //{
        //        //    responseDetail.ResponseValue = "No UnEncrypted Barcodes Found.";
        //        //}
        //        //else
        //        {
        //            var unbarcode = 0;
        //            var currentRecord = 0;

        //            while (count >= currentRecord)
        //            {
        //                var codes = await Task.Run(() => entity.R_BarcodeMaster.Where(b => !(b.IsEncrypted == true)).OrderBy(b => b.Sno).Skip(currentRecord).Take(50).ToList());
        //                //var codes = unEncryptedBarcodes;
        //                var salt = ConfigurationManager.AppSettings["Salt"];
        //                foreach (var code in codes)
        //                {
        //                    if (code.IsEncrypted != true)
        //                    {
        //                        var encrCode = EncodeBarcode(code.Barcode, salt);
        //                        code.Barcode = encrCode;
        //                        code.IsEncrypted = true;
        //                        unbarcode++;
        //                    }

        //                    currentRecord++;
        //                }
        //                await entity.SaveChangesAsync();
        //            }
        //            responseDetail.Status = true;
        //            responseDetail.ResponseValue = unbarcode + " Barcodes Encrypted.";
        //        }

        //        return responseDetail;
        //    }
        //    catch (Exception e)
        //    {
        //        return new Response
        //        {
        //            ResponseValue = e.Message + DateTime.Now.ToString()
        //        };
        //        // return e.Message.ToString();
        //    }
        //}

        public async Task<Response> EncryptBarcodes()
        {
            try
            {
                Response responseDetail = new Response();

                var unEncryptedBarcodes = await Task.Run(() => entity.R_BarcodeMaster.Where(b => !(b.IsEncrypted == true)));

                var count = unEncryptedBarcodes.Count();
                if (unEncryptedBarcodes == null || count == 0)
                {
                    responseDetail.ResponseValue = "No UnEncrypted Barcodes Found.";
                }
                else
                {
                    var unbarcode = 0;
                    var currentRecord = 0;

                    while (count > currentRecord)
                    {
                        var codes = unEncryptedBarcodes.OrderBy(b => b.Sno).Skip(currentRecord).Take(500).ToList();
                        var salt = ConfigurationManager.AppSettings["Salt"];
                        foreach (var code in codes)
                        {
                            if (code.IsEncrypted != true)
                            {
                                var encrCode = EncodeBarcode(code.Barcode, salt);
                                code.Barcode = encrCode;
                                code.IsEncrypted = true;
                                unbarcode++;
                            }

                            currentRecord++;
                        }
                        await entity.SaveChangesAsync();
                    }
                    responseDetail.Status = true;
                    responseDetail.ResponseValue = unbarcode + " Barcodes Encrypted.";
                }

                return responseDetail;
            }
            catch (Exception e)
            {
                return new Response
                {
                    ResponseValue = e.Message + DateTime.Now.ToString()
                };
                // return e.Message.ToString();
            }
        }

        public static string EncodeBarcode(string pass, string salt) //encrypt password    
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);    
            return EncodeBarcodeMd5(Convert.ToBase64String(inArray));
        }

        public static string EncodeBarcodeMd5(string pass) //Encrypt using MD5    
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }

        private async Task<int> CartProductCount(string retailerId)
        {
            var cartList = await Task.Run(() => entity.R_CartDetail.Where(p => p.RetailerId == retailerId));
            return cartList.Count();
        }

        private async Task<decimal> GetCurrentBalacePoints(IEnumerable<R_PointsLedger> pList, string retailerId)
        {
            var dabitPoints = await Task.Run(() => pList.Where(d => d.RetailerId == retailerId).Sum(p => p.DabitPoints));
            var creditPoints = await Task.Run(() => pList.Where(d => d.RetailerId == retailerId).Sum(p => p.CreditPoints));
            var points = ((dabitPoints ?? 0) - (creditPoints ?? 0));
            return points;
        }

        private double FindDistance(string shopX, string shopY, string currentGpsX, string currentGpsY)
        {
            var sCoord = new GeoCoordinate(Convert.ToDouble(shopX), Convert.ToDouble(shopY));
            var eCoord = new GeoCoordinate(Convert.ToDouble(currentGpsX), Convert.ToDouble(currentGpsY));

            var distance = sCoord.GetDistanceTo(eCoord);

            return distance;

        }

        public async Task SendMessage(string mobileNo, string MessageBody)
        {
            var msgUrl = "http://49.50.77.216/API/SMSHttp.aspx?UserId=ttretailer&pwd=TTrlr456&Message={MessageBody}&Contacts={ContactsValue}&SenderId=TTRtlr";
            
            msgUrl = msgUrl.Replace("{MessageBody}", MessageBody).Replace("{ContactsValue}", mobileNo);

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(msgUrl);

                // If the response contains content we want to read it!
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task AssignPointsForPromotionApproval(string retailerId, string promotionId, int? points, string promoHeading)
        {
            try
            {
                var noOfEntriesRegistered = await Task.Run(() => entity.R_PromotionEntries.Where(o => o.RetailerId == retailerId && o.PromotionId == promotionId));
                var isApproved = noOfEntriesRegistered.All(p => p.IsValid == true);
                if (isApproved)
                {
                    var entry = await Task.Run(() => entity.R_PointsLedger.FirstOrDefault(o => o.RetailerId == retailerId && !string.IsNullOrEmpty(o.Barcode) && o.Barcode.Contains(promotionId)));

                    if (entry == null)
                    {
                        /***Add Points on approval***/
                        if (points != null && points > 0)
                        {
                            var pointsDetail = new R_PointsLedger();
                            pointsDetail.Id = Guid.NewGuid().ToString().Substring(0, 5);
                            pointsDetail.Barcode = "Promotion Entry Approved For :: " + promotionId + " # " + promoHeading;
                            pointsDetail.DabitPoints = points;
                            pointsDetail.EarnSpentDate = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                            pointsDetail.RetailerId = retailerId;
                            pointsDetail.ProductQty = 0;
                            entity.R_PointsLedger.Add(pointsDetail);
                            // await entity.SaveChangesAsync();
                        }

                        await entity.SaveChangesAsync();
                        var pointsList = await Task.Run(() => entity.R_PointsLedger.Where(p => p.RetailerId == retailerId));
                        var data = await Task.Run(() => entity.R_RetailerMaster.FirstOrDefault(o => o.ID == retailerId));
                        data.Points = await this.GetCurrentBalacePoints(pointsList, data.ID);
                        await entity.SaveChangesAsync();
                    }
                }
                /**************End***************/
            }
            catch (Exception e)
            {

            }
        }

        public async Task<decimal?> CheckPoints(R_PointsLedger detail)
        {
            try
            {
                var today = DateTime.Now;

                var list = await Task.Run(() => entity.R_FestivePointMaster.Where(p => p.ToDate.Value.Year < today.Year && p.ToDate.Value.Month < today.Month && p.ToDate.Value.Day < today.Day).ToList());

                if (list != null && list.Count > 0)
                {
                    foreach (var d in list)
                    {
                        d.IsActive = false;
                    }

                    await this.entity.SaveChangesAsync();
                }

                var pointsEntry = await Task.Run(() => entity.R_FestivePointMaster.FirstOrDefault(p => p.FromDate.Value.Year <= detail.EarnSpentDate.Value.Year && p.FromDate.Value.Month <= detail.EarnSpentDate.Value.Month && p.FromDate.Value.Day <= detail.EarnSpentDate.Value.Day &&
                p.ToDate.Value.Year >= detail.EarnSpentDate.Value.Year && p.ToDate.Value.Month >= detail.EarnSpentDate.Value.Month && p.ToDate.Value.Day >= detail.EarnSpentDate.Value.Day && p.FromPoint == detail.DabitPoints));
                if (pointsEntry == null)
                {
                    return detail.DabitPoints;
                }
                else
                {
                    if (pointsEntry.IsActive == false)
                    {
                        return detail.DabitPoints;
                    }
                    else
                    {
                        if (pointsEntry.FromPoint == detail.DabitPoints)
                        {
                            return pointsEntry.ToPoint;
                        }
                        else
                        {
                            return detail.DabitPoints;
                        }
                    }
                }

                /**************End***************/
            }
            catch (Exception e)
            {
                return detail.DabitPoints;
            }
        }

        public async Task<Response> AssignRetailerFields()
        {
            try
            {
                Response responseDetail = new Response();
                //var retailers = await Task.Run(() => entity.R_RetailerMaster.ToList());
                //if (retailers == null)
                //{
                //    responseDetail.ResponseValue = "Retailer not found.";
                //}
                //else
                {
                    var retailerList = await Task.Run(() => entity.R_RetailerMaster.ToList());
                    var productList = await Task.Run(() => entity.R_ProductMaster.ToList());
                    var pointsList = await Task.Run(() => entity.R_PointsLedger.Where(p => p.FirmName == null).Take(500));
                    foreach (var data in pointsList)
                    {
                        var retailer = await Task.Run(() => retailerList.FirstOrDefault(d => d.ID == data.RetailerId));
                        if (retailer != null)
                        {
                            data.FirmName = retailer.FirmName;
                        }

                        var product = await Task.Run(() => productList.FirstOrDefault(d => d.Id == data.ProductId));
                        if (product != null)
                        {
                            data.ProductName = product.Name;
                        }

                        //data.EarnSpentDateString = data.EarnSpentDate.ToString();
                    }

                    await entity.SaveChangesAsync();
                }

                responseDetail.Status = true;
                responseDetail.ResponseValue = "Password has been sent to your registered mobile no.";

                return responseDetail;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }
    }
}