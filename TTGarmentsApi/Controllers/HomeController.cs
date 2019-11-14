using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TTGarmentsApi.Models;
using TTGarmentsApi.Repository;

namespace TTGarmentsApi.Controllers
{
    public class HomeController : ApiController
    {
        public HomeRepository repository;

        [HttpPost, Route("api/Home/ManageRetailer/{operation}")]
        public async Task<IHttpActionResult> ManageRetailer(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_RetailerMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageRetailer(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageAdminUser/{operation}")]
        public async Task<IHttpActionResult> ManageAdminUser(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_AdminMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageAdminUser(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManagePromotion/{operation}")]
        public async Task<IHttpActionResult> ManagePromotion(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_Promotion>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManagePromotion(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageMessages/{operation}")]
        public async Task<IHttpActionResult> ManageMessages(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_MessageMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageMessages(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageAppVersion/{operation}")]
        public async Task<IHttpActionResult> ManageAppVersion(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_AppVersion>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageAppVersion(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageFetivePoints/{operation}")]
        public async Task<IHttpActionResult> ManageFetivePoints(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_FestivePointMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageFetivePoints(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/LoginAdminUser")]
        public async Task<IHttpActionResult> LoginAdminUser()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_AdminMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.LoginAdminUser(filters.Username, filters.Password);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/LoginRetailer")]
        public async Task<IHttpActionResult> LoginRetailer()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_RetailerMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.LoginRetailer(filters.Mobile, filters.Password);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/StateList")]
        public async Task<IHttpActionResult> StateList()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_StateMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.StateList();
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/CityList/{stateId}")]
        public async Task<IHttpActionResult> CityList(int stateId)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_StateMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.CityList(stateId);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet, Route("api/Home/GetDistributerCityList")]
        public async Task<IHttpActionResult> GetDistributerCityList()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            this.repository = new HomeRepository();
            var result = await this.repository.GetDistributerCityList();
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet, Route("api/Home/GetDistributerByCityName/{cityName}")]
        public async Task<IHttpActionResult> GetDistributerByCityName(string cityName)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            this.repository = new HomeRepository();
            var result = await this.repository.GetDistributerByCityName(cityName);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet, Route("api/Home/AddAppDownloadDetail/{deviceId}")]
        public async Task<IHttpActionResult> AddAppDownloadDetail(string deviceId)
        {
            this.repository = new HomeRepository();
            var result = await this.repository.AddAppDownloadDetail(deviceId);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet, Route("api/Home/ListAppDownloadDetail")]
        public async Task<IHttpActionResult> ListAppDownloadDetail()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            this.repository = new HomeRepository();
            var result = await this.repository.ListAppDownloadDetail();
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        // Post api/values
        [HttpPost, Route("api/Home/SaveImage")]
        public async Task<IHttpActionResult> SaveImage()
        {
            repository = new HomeRepository();
            var detail1 = await Request.Content.ReadAsStringAsync();
            var detail = JsonConvert.DeserializeObject<ProfilePhoto>(detail1);

            var result = await this.repository.SaveImage(detail);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        // Post api/values
        [HttpGet, Route("api/Home/EncryptBarcodes")]
        public async Task<IHttpActionResult> EncryptBarcodes()
        {
            repository = new HomeRepository();
            //            var result = await this.repository.EncryptBarcodes();
            var result = await this.repository.AssignRetailerFields();
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        // Post api/values
        [HttpPost, Route("api/Home/ScanBarcode")]
        public async Task<IHttpActionResult> ScanBarcode()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<BarcodeDetail>(detail);

            repository = new HomeRepository();
            //var detail1 = await Request.Content.ReadAsStringAsync();
            //var detail = JsonConvert.DeserializeObject<BarcodeDetail>(filters);
            var result = await this.repository.ScanBarcode(filters);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageMediaCategory/{operation}")]
        public async Task<IHttpActionResult> ManageMediaCategory(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_MediaCategory>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageMediaCategory(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManagePromotionEntry/{operation}")]
        public async Task<IHttpActionResult> ManagePromotionEntry(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<PromotionEntryDetail>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManagePromotionEntry(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageBanner/{operation}")]
        public async Task<IHttpActionResult> ManageBanner(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_BannerMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageBanner(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }


        [HttpPost, Route("api/Home/ManageUploadedMedia/{operation}")]
        public async Task<IHttpActionResult> ManageUploadedMedia(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_UploadedMedia>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageUploadedMedia(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManagePoints/{operation}")]
        public async Task<IHttpActionResult> ManagePoints(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_PointsLedger>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManagePoints(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManagePointsWithFilter")]
        public async Task<IHttpActionResult> ManagePointsWithFilter()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<Filters>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManagePointsWithFilter(filters);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageRetailerWithFilter")]
        public async Task<IHttpActionResult> ManageRetailerWithFilter()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<Filters>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageRetailerWithFilter(filters);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }


        [HttpPost, Route("api/Home/ManageProducts/{operation}")]
        public async Task<IHttpActionResult> ManageProducts(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<List<R_ProductMaster>>(detail);
            this.repository = new HomeRepository();

            var result = await this.repository.ManageProducts(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet, Route("api/Home/GetDashboardCounts")]
        public async Task<IHttpActionResult> GetDashboardCounts()
        {
            this.repository = new HomeRepository();
            var result = await this.repository.GetDashboardCounts();
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet, Route("api/Home/ForgotPassword/{MobileNo}")]
        public async Task<IHttpActionResult> ForgotPassword(string MobileNo)
        {
            this.repository = new HomeRepository();
            var result = await this.repository.ForgotPassword(MobileNo);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/GetProductWithFilter")]
        public async Task<IHttpActionResult> GetProductWithFilter()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<Filters>(detail);
            this.repository = new HomeRepository();

            var result = await this.repository.GetProductWithFilter(filters);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageOrder/{operation}")]
        public async Task<IHttpActionResult> ManageOrder(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<OrderRequest>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageOrder(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/ManageNotification/{operation}")]
        public async Task<IHttpActionResult> ManageNotification(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_NotificationManager>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageNotification(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost, Route("api/Home/UpdateStatus/")]
        public async Task<IHttpActionResult> UpdateStatus()
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_OrderMaster>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.UpdateStatus(filters);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        
        [HttpPost, Route("api/Home/ManageSetting/{operation}")]
        public async Task<IHttpActionResult> ManageSetting(string operation)
        {
            var detail = await Request.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<R_MasterSetting>(detail);
            this.repository = new HomeRepository();
            var result = await this.repository.ManageSetting(filters, operation);
            return Content(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }
    }
}