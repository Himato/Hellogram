using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace HelloGram.Core.ViewModels
{
    public class UserViewModel
    {
        public bool IsOther { get; set; }

        public string Id { get; set; }

        public string Username { get; set; }

        public string Image { get; set; }

        public string Email { get; set; }

        [Required, MinLength(4, ErrorMessage = "Name can't be less than 4 letters"), MaxLength(32, ErrorMessage = "Name can't be more than 32 letters")]
        [RegularExpression(@"^[a-zA-Z_\s]+$", ErrorMessage = "Name can only contain English letters, underscores, and spaces")]
        public string Name { get; set; }

        [RegularExpression(@"^[\d]{4}/[\d]{1,2}/[\d]{1,2}$", ErrorMessage = "Birthday must be in this format yyyy/mm/dd")]
        public string Birthday { get; set; }

        [MaxLength(32, ErrorMessage = "City name can't be more than 32 letters")]
        public string City { get; set; }

        [MaxLength(32, ErrorMessage = "Country name can't be more than 32 letters")]
        public string Country { get; set; }

        [DisplayName("Phone Number")]
        [RegularExpression(@"^[\+\-\d]+$", ErrorMessage = "Phone Number must be in this format +1-202-555-0146")]
        public string PhoneNumber { get; set; }

        [MaxLength(64, ErrorMessage = "University name can't be more than 64 letters")]
        public string University { get; set; }

        [MaxLength(32, ErrorMessage = "Country name can't be more than 32 letters")]
        public string Company { get; set; }

        [MaxLength(32, ErrorMessage = "Country name can't be more than 32 letters")]
        public string Position { get; set; }

        [MaxLength(255, ErrorMessage = "Country name can't be more than 32 letters")]
        public string About { get; set; }

        [DisplayName("Facebook Link")]
        [RegularExpression(@"https?:\/\/(www\.)?facebook\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&/=]*)", ErrorMessage = "Invalid Facebook Url")]
        public string FacebookLink { get; set; }

        [DisplayName("Twitter Link")]
        [RegularExpression(@"https?:\/\/(www\.)?twitter\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&/=]*)", ErrorMessage = "Invalid Twitter Url")]
        public string TwitterLink { get; set; }

        public string NumberOfPosts { get; set; }

        public string NumberOfFollowers { get; set; }

        public string NumberOfFollowings { get; set; }

        public bool IsFollowing { get; set; }

        public bool HasMessages { get; set; }

        public int NumberOfNotifications { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        public int? Hits { get; set; }

        public static UserViewModel Create(ApplicationUser user, string loggedUserId, bool loadProfileData = false, bool isAdmin = false)
        {
            var repository = new Repository();
            var isOther = !user.Id.Equals(loggedUserId);

            var login = isAdmin ? repository.ApplicationUserRepository.GetLogin(user.Id) : null;

            var userViewModel = new UserViewModel
            {
                IsOther = isOther,
                Id = user.Id,
                Username = user.UserName,
                Image = user.GetImage(),
                Email = user.Email,
                Name = user.Name,
                Birthday = user.Age?.ToString("yyyy/MM/dd"),
                City = user.City,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                University = user.University,
                Company = user.Company,
                Position = user.Position,
                About = user.About,
                FacebookLink = user.FacebookLink,
                TwitterLink = user.TwitterLink,
                IsFollowing = isOther && repository.ApplicationUserRepository.IsFollowing(loggedUserId, user.Id),
                NumberOfNotifications = repository.ApplicationUserRepository.GetNumberOfUnreadNotifications(user.Id),
                HasMessages = repository.MessengerRepository.HasUnreadMessages(user.Id),
                CreatedAt = user.CreatedAt,
                LastLogin = login?.DateTime,
                Hits = login?.Count
            };

            if (loadProfileData)
            {
                var followers = repository.ApplicationUserRepository.GetNumberOfFollowers(user.Id);
                userViewModel.NumberOfFollowers = (followers >= 1000) ? $"{(followers / 1000.00):F1}K" : $"{followers}";

                var posts = repository.PostRepository.GetNumberOfPosts(user.Id);
                userViewModel.NumberOfPosts = (posts >= 1000) ? $"{(posts / 1000.00):F1}K" : $"{posts}";

                var followings = repository.ApplicationUserRepository.GetNumberOfFollowings(user.Id);
                userViewModel.NumberOfFollowings = (followings >= 1000) ? $"{(followings / 1000.00):F1}K" : $"{followings}";
            }

            return userViewModel;
        }

        public void Update(ApplicationUser user)
        {
            Username = user.UserName;
            Email = user.Email;

            user.Name = Name;
            try
            {
                user.Age = DateTime.Parse(Birthday);
            }
            catch
            {
                user.Age = null;
            }
            user.City = City;
            user.Country = Country;
            user.PhoneNumber = PhoneNumber;
            user.University = University;
            user.Company = Company;
            user.Position = Position;
            user.About = About;
            user.FacebookLink = FacebookLink;
            user.TwitterLink = TwitterLink;

            var repository = new Repository();
            var followers = repository.ApplicationUserRepository.GetNumberOfFollowers(user.Id);
            NumberOfFollowers = (followers >= 1000) ? $"{(followers / 1000.00):F1}K" : $"{followers}";

            var posts = repository.PostRepository.GetNumberOfPosts(user.Id);
            NumberOfPosts = (posts >= 1000) ? $"{(posts / 1000.00):F1}K" : $"{posts}";

            var followings = repository.ApplicationUserRepository.GetNumberOfFollowings(user.Id);
            NumberOfFollowings = (followings >= 1000) ? $"{(followings / 1000.00):F1}K" : $"{followings}";
        }

        public int GetAge()
        {
            var birthdate = DateTime.Parse(Birthday);
            // Save today's date.
            var today = DateTime.Today;
            // Calculate the age.
            var age = today.Year - birthdate.Year;
            // Go back to the year the person was born in case of a leap year
            if (birthdate.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}