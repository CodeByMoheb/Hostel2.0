using Microsoft.AspNetCore.Identity;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hostel2._0.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roleNames = { "Admin", "HostelManager", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create or update admin user
            var adminEmail = "mohebullah.cse@gmail.com";
            var adminPassword = "Admin1234@";
            var oldAdminEmail = "admin@hostel.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            var oldAdminUser = await userManager.FindByEmailAsync(oldAdminEmail);

            if (adminUser == null && oldAdminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                var userToUpdate = adminUser ?? oldAdminUser;
                if (userToUpdate == null)
                {
                    throw new InvalidOperationException("Failed to find or create admin user");
                }
                
                if (userToUpdate.Email != adminEmail)
                {
                    userToUpdate.Email = adminEmail;
                    userToUpdate.UserName = adminEmail;
                    await userManager.UpdateAsync(userToUpdate);
                }
                // Remove old password and set new one
                var token = await userManager.GeneratePasswordResetTokenAsync(userToUpdate);
                await userManager.ResetPasswordAsync(userToUpdate, token, adminPassword);
                if (!await userManager.IsInRoleAsync(userToUpdate, "Admin"))
                {
                    await userManager.AddToRoleAsync(userToUpdate, "Admin");
                }
            }

            // Create a test manager if it doesn't exist
            var managerEmail = "manager@hostel.com";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);

            if (managerUser == null)
            {
                var manager = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true,
                    FirstName = "Test",
                    LastName = "Manager"
                };

                var result = await userManager.CreateAsync(manager, "Manager@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "HostelManager");

                    // Create a test hostel for the manager
                    var hostel = new Hostel
                    {
                        Name = "Test Hostel",
                        Address = "123 Test Street",
                        PhoneNumber = "1234567890",
                        Email = "test@hostel.com",
                        ManagerId = manager.Id,
                        IsActive = true,
                        SubscriptionStartDate = DateTime.Now,
                        SubscriptionEndDate = DateTime.Now.AddYears(1)
                    };

                    context.Hostels.Add(hostel);
                    await context.SaveChangesAsync();
                }
            }

            // Create a test student if it doesn't exist
            var studentEmail = "student@hostel.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                var student = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    EmailConfirmed = true,
                    FirstName = "Test",
                    LastName = "Student"
                };

                var result = await userManager.CreateAsync(student, "Student@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, "Student");

                    // Create a test student profile
                    var studentProfile = new Student
                    {
                        Name = "Test Student",
                        StudentId = "STU001",
                        Email = studentEmail,
                        PhoneNumber = "9876543210",
                        DateOfBirth = new DateTime(2000, 1, 1),
                        Gender = Gender.Male,
                        Address = "456 Student Street",
                        IsActive = true,
                        HostelId = context.Hostels.First().Id,
                        UserId = student.Id
                    };

                    context.Students.Add(studentProfile);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 