﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TTGarmentsApi.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TTLimitedEntities : DbContext
    {
        public TTLimitedEntities()
            : base("name=TTLimitedEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<R_AdminMaster> R_AdminMaster { get; set; }
        public virtual DbSet<R_CityMaster> R_CityMaster { get; set; }
        public virtual DbSet<R_StateMaster> R_StateMaster { get; set; }
        public virtual DbSet<R_DistributerMaster> R_DistributerMaster { get; set; }
        public virtual DbSet<R_AppDownloadDetail> R_AppDownloadDetail { get; set; }
        public virtual DbSet<R_BannerMaster> R_BannerMaster { get; set; }
        public virtual DbSet<R_MediaCategory> R_MediaCategory { get; set; }
        public virtual DbSet<R_NotificationReply> R_NotificationReply { get; set; }
        public virtual DbSet<R_UploadedMedia> R_UploadedMedia { get; set; }
        public virtual DbSet<R_BarcodeMaster> R_BarcodeMaster { get; set; }
        public virtual DbSet<R_PointsLedger> R_PointsLedger { get; set; }
        public virtual DbSet<R_CartDetail> R_CartDetail { get; set; }
        public virtual DbSet<R_Promotion> R_Promotion { get; set; }
        public virtual DbSet<R_ProductMaster> R_ProductMaster { get; set; }
        public virtual DbSet<R_PromotionEntries> R_PromotionEntries { get; set; }
        public virtual DbSet<R_AppVersion> R_AppVersion { get; set; }
        public virtual DbSet<R_OrderMaster> R_OrderMaster { get; set; }
        public virtual DbSet<R_NotificationResult> R_NotificationResult { get; set; }
        public virtual DbSet<R_NotificationManager> R_NotificationManager { get; set; }
        public virtual DbSet<R_FestivePointMaster> R_FestivePointMaster { get; set; }
        public virtual DbSet<R_MessageMaster> R_MessageMaster { get; set; }
        public virtual DbSet<R_MasterSetting> R_MasterSetting { get; set; }
        public virtual DbSet<R_RetailerMaster> R_RetailerMaster { get; set; }
    }
}
