﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using System.Threading;

namespace BasicFacebookFeatures
{
    class FacebookDataFetcher : IFacebookDataFetcher
    {
        private readonly User m_LoggedInUser;
        private static volatile FacebookDataFetcher m_Instance;
        private static readonly object r_Lock = new object();

        private FacebookDataFetcher(User i_LoggedInUser)
        {
            m_LoggedInUser = i_LoggedInUser;
        }

        public static FacebookDataFetcher GetInstance(User i_LoggedInUser)
        {
            if (m_Instance == null || (m_Instance.m_LoggedInUser != i_LoggedInUser))
            {
                lock (r_Lock)
                {
                    if (m_Instance == null || (m_Instance.m_LoggedInUser != i_LoggedInUser))
                    {
                        m_Instance = new FacebookDataFetcher(i_LoggedInUser);
                    }
                }
            }
            return m_Instance;
        }

        public FacebookObjectCollection<Page> FetchLikedPages()
        {
            return m_LoggedInUser.LikedPages;//.ToList();
        }

        public FacebookObjectCollection<Post> FetchPosts()
        {
            return m_LoggedInUser.Posts;//.ToList();
        }

        public FacebookObjectCollection<Album> FetchAlbums()
        {
            return m_LoggedInUser.Albums;
        }

        public string FetchBasicInfo()
        {
            string basicDetails = "Name: " + m_LoggedInUser.FirstName + " " + m_LoggedInUser.LastName + "\n\n";

            basicDetails += fetchBirthdayAndCalculateCountdown();
            basicDetails += "Gender: " + m_LoggedInUser.Gender + "\n\n";
            basicDetails += "Email: " + m_LoggedInUser.Email + "\n\n";

            return basicDetails;
        }

        private string fetchBirthdayAndCalculateCountdown()
        {
            string res = "";
            try
            {
                string userBirthday = m_LoggedInUser.Birthday;
                DateTime today = DateTime.Today;
                DateTime formatedUserBirthday;
                if (DateTime.TryParseExact(userBirthday, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out formatedUserBirthday))
                {
                    DateTime birthdayThisYear = new DateTime(today.Year, formatedUserBirthday.Month, formatedUserBirthday.Day);
                    if (today > birthdayThisYear)
                    {
                        birthdayThisYear = birthdayThisYear.AddYears(1);
                    }

                    TimeSpan daysDifference = birthdayThisYear.Subtract(today);
                    if (daysDifference.Days == 0)
                    {
                        res += "Happy birthday!!!\n\n";
                    }
                    else
                    {
                        res += $"Your birthday is in {userBirthday}\n\nYou have {daysDifference.Days} days until your birthday\n\n";
                    }
                }
                else
                {
                    res += "You havent provided a birthday\n\n";

                }
            }
            catch (Exception generalException)
            {
                throw new Exception("Error trying to fetch birthday");
            }
          
            return res;
        }

        public string FetchProfilePicURL()
        {
            return m_LoggedInUser.PictureNormalURL;
        }

        public Status PostStatus(string i_Text)
        {
            return m_LoggedInUser.PostStatus(i_Text);
        }

        public FacebookObjectCollection<User> FetchFriends()
        {
            return m_LoggedInUser.Friends; 
        }
    }
}
