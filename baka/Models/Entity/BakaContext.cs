using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace baka.Models.Entity
{
    public class BakaContext : DbContext
    {
        public DbSet<BakaUser> Users { get; set; }

        public DbSet<BakaFile> Files { get; set; }

        public DbSet<BakaLink> Links { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder OptionsBuilder)
        {
            OptionsBuilder.UseSqlite($"Data Source = {Globals.Config.DbName}");
        }
    }

    public class BakaUser
    {
        public BakaUser()
        {
            Files = new HashSet<BakaFile>();
            Links = new HashSet<BakaLink>();
        }

        public string Name { get; set; }

        public string Username { get; set; }

        public int Id { get; set; }

        [JsonIgnore]
        public ICollection<BakaFile> Files { get; set; }

        [JsonIgnore]
        public ICollection<BakaLink> Links { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        [JsonProperty("timestamp")]
        public string Epoch { get { return Timestamp.ToFileTimeUtc().ToString(); } }

        public string Token { get; set; }

        public string Email { get; set; }

        public string InitialIp { get; set; }

        public double UploadLimitMB { get; set; }

        public bool Deleted { get; set; }

        public bool Disabled { get; set; }

        public string AccountType { get; set; }
    }

    public class BakaFile
    {
        [JsonProperty("backend_file_id")]
        public string BackendFileId { get; set; }

        [JsonProperty("db_id")]
        public int Id { get; set; }

        [JsonProperty("result_code")]
        public string ExternalId { get; set; }

        [JsonProperty("file_name")]
        public string Filename { get; set; }

        [JsonProperty("ext")]
        public string Extension { get; set; }

        [JsonProperty("ip")]
        public string IpUploadedFrom { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("uploader")]
        [JsonIgnore]
        public BakaUser Uploader { get; set; }

        [JsonProperty("uploader_id")]
        [NotMapped]
        public int BakaUploaderId { get { return Uploader.Id; } }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        [JsonProperty("file_size")]
        public double FileSizeMB { get; set; }

        [JsonProperty("timestamp")]
        [NotMapped]
        public string Epoch { get { return Timestamp.ToFileTimeUtc().ToString(); } }

        [NotMapped]
        [JsonProperty("content_type")]
        public string ContentType
        {
            get
            {
                return BakaMime.GetMimeType(Extension);
            }
        }
    }

    public class BakaLink
    {
        [JsonProperty("dest")]
        public string Destination { get; set; }

        [JsonProperty("ip")]
        public string UploadedFromIp { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("db_id")]
        public int Id { get; set; }

        [JsonProperty("uploader")]
        [JsonIgnore]
        public BakaUser Uploader { get; set; }

        [JsonProperty("uploader_id")]
        [NotMapped]
        public int BakaUploaderId { get { return Uploader.Id; } }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        [JsonProperty("timestamp")]
        [NotMapped]
        public string Epoch { get { return Timestamp.ToFileTimeUtc().ToString(); } }
    }
}