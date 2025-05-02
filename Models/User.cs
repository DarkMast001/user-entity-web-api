using System.Text.RegularExpressions;

namespace UserEntityWebAPI.Models
{
    public class User
    {
        public Guid Id { get; }
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Name { get; private set; }
        public int Gender { get; private set; }
        public DateTime? Birthday { get; private set; }
        public bool IsAdmin { get; private set; }

        public DateTime CreatedOn { get; }
        public string CreatedBy { get; }

        public DateTime? ModifiedOn { get; private set; }
        public string ModifiedBy { get; private set; }

        public DateTime? RevokedOn { get; private set; }
        public string RevokedBy { get; private set; }

        public User(string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string createdBy)
        {
            Id = Guid.NewGuid();
            Login = login;
            Password = password;
            Name = name;
            Gender = gender;
            Birthday = birthday;
            IsAdmin = isAdmin;

            CreatedOn = DateTime.Now;
            CreatedBy = createdBy;

            ModifiedBy = "";
            RevokedBy = "";
        }

        public static User CreateAdmin()
        {
            return new User("admin", "admin", "Administrator", 0, null, true, "system");
        }

        public void UpdateName(string newName, string modifiedBy)
        {
            Name = newName;
            ModifiedOn = DateTime.Now;
            ModifiedBy = modifiedBy;
        }

        public void UpdatePassword(string newPassword, string modifiedBy)
        {
            Password = newPassword;
            ModifiedOn = DateTime.Now;
            ModifiedBy = modifiedBy;
        }

        public void UpdateLogin(string newLogin, string modifiedBy)
        {
            Login = newLogin;
            ModifiedOn = DateTime.Now;
            ModifiedBy = modifiedBy;
        }

        public void UpdateGender(int newGender, string modifiedBy)
        {
            Gender = newGender;
            ModifiedOn = DateTime.Now;
            ModifiedBy = modifiedBy;
        }

        public void UpdateBirthday(DateTime newBirthday, string modifiedBy)
        {
            Birthday = newBirthday;
            ModifiedOn = DateTime.Now;
            ModifiedBy = modifiedBy;
        }

        public void Revoke(string revokedBy)
        {
            RevokedOn = DateTime.Now;
            RevokedBy = revokedBy;
        }

        public void Restore()
        {
            RevokedOn = null;
            RevokedBy = "";
        }

        public bool IsActive => (RevokedOn == null);
    }
}
