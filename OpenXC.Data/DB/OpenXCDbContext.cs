using OpenXC.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Data.DB
{
    /// <summary>
    /// OpenXC database context.
    /// </summary>
    public class OpenXCDbContext : DbContext
    {
        /// <summary>
        /// Web users.
        /// </summary>
        public virtual DbSet<WebUser> WebUsers { get; set; }

        /// <summary>
        /// Logging devices.
        /// </summary>
        public virtual DbSet<LoggingDevice> LoggingDevices { get; set; }

        /// <summary>
        /// Firmware upgrades.
        /// </summary>
        public virtual DbSet<FirmwareUpgrade> FirmwareUpgrades { get; set; }

        /// <summary>
        /// Logged data.
        /// </summary>
        public virtual DbSet<LoggedData> LoggedData { get; set; }


        /// <summary>
        /// Create a database context with the default connection string name: "OpenXCDbEntities".
        /// </summary>
        public OpenXCDbContext()
            : base("OpenXCDbEntities")
        {
        }

        /// <summary>
        /// Create a new context.
        /// </summary>
        /// <returns>A new database context.</returns>
        public static OpenXCDbContext Create()
        {
            return new OpenXCDbContext();
        }

        /// <summary>
        /// Set up data model.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // We want singular table names.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Configure web users.
            modelBuilder.Entity<WebUser>()
                .Property(dbWebUser => dbWebUser.UserName)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute() { IsUnique = true }));
            modelBuilder.Entity<WebUser>()
                .Property(dbWebUser => dbWebUser.PasswordHash)
                .IsRequired();

            // Configure logging devices.
            modelBuilder.Entity<LoggingDevice>()
                .Property(dbLoggingDevice => dbLoggingDevice.DeviceName)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute() { IsUnique = true }));
            modelBuilder.Entity<LoggingDevice>()
                .Property(dbLoggingDevice => dbLoggingDevice.FriendlyName)
                .IsRequired()
                .HasMaxLength(64);
            modelBuilder.Entity<LoggingDevice>()
                .HasOptional(dbLoggingDevice => dbLoggingDevice.FirmwareUpgrade)
                .WithMany(dbFirmwareUpgrade => dbFirmwareUpgrade.LoggingDevices);

            // Configure firmware upgrades
            modelBuilder.Entity<FirmwareUpgrade>()
                .Property(dbFirmwareUpgrade => dbFirmwareUpgrade.Name)
                .IsRequired()
                .HasMaxLength(256);
            modelBuilder.Entity<FirmwareUpgrade>()
                .Property(dbFirmwareUpgrade => dbFirmwareUpgrade.FileHash)
                .IsRequired();
            modelBuilder.Entity<FirmwareUpgrade>()
                .Property(dbFirmwareUpgrade => dbFirmwareUpgrade.FileData)
                .IsRequired();

            // Configure logged data
            modelBuilder.Entity<LoggedData>()
                .Property(dbLoggedData => dbLoggedData.LoggedTime)
                .IsRequired()
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
            modelBuilder.Entity<LoggedData>()
                .HasRequired(dbLoggedData => dbLoggedData.LoggingDevice)
                .WithMany(dbLoggingDevice => dbLoggingDevice.LoggedData);
        }
    }
}
