namespace sysmed.Model
{
    public class UserBasicInfo
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public int? RoleID { get; set; }
        public string Email { get; set; }
        public string FullName
        {
            get => $"{FirstName} {LastName}";
        }

        public string RoleText { get; set; }
        //public int UserId { get; set; }
        public int UserTypeId { get; set; }

        public string Picture { get; set; }

        //public UserType UserType { get; set; }
        public string Rnc { get; set; }
        public int StatusId { get; set; }
        public string Roles { get; set; }
        public int AuthorId { get; set; }
        public int? ShopId { get; set; }

        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public DateTime TokenExpires { get; set; }

        public string Password { get; set; }

        public bool IsRemembered { get; set; }

        public string FullPicture
        {
            get
            {
                if (string.IsNullOrEmpty(Picture))
                {
                    return "avatar_user.png";
                }
                return UserTypeId == 1 ? string.Format("http://torneoprediccionesapi.azurewebsites.net/{0}", Picture.Substring(1)) : Picture;
            }
        }

        public byte[] ImageArray { get; internal set; }

        //public override int GetHashCode()
        //{
        //    return UserId;
        //}
    }

    public enum RoleDetails
    {
        Student = 1,
        Teacher,
        Admin,
    }
}
