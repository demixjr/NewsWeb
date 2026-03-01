using BLL.DTO;
using PL.Helpers;
using UI.Tests.Helpers;
using Xunit;

namespace UI.Tests
{
    public class SessionHelperTests
    {
        private readonly TestSession _session = new();

        [Fact]
        public void SetCurrentUser_ValidUser_UserIsStored()
        {
            var user = new UserDTO { Role = "Admin" };
            SessionUserHelper.SetCurrentUser(_session, user);
            Assert.NotNull(SessionUserHelper.GetCurrentUser(_session));
        }

        [Fact]
        public void ClearCurrentUser_RemovesUser()
        {
            SessionUserHelper.SetCurrentUser(_session, new UserDTO { Role = "Admin" });
            SessionUserHelper.ClearCurrentUser(_session);
            Assert.Null(SessionUserHelper.GetCurrentUser(_session));
        }

        [Fact]
        public void IsAuthenticated_WithUser_ReturnsTrue()
        {
            SessionUserHelper.SetCurrentUser(_session, new UserDTO { Role = "Admin" });
            Assert.True(SessionUserHelper.IsAuthenticated(_session));
        }

        [Fact]
        public void IsAuthenticated_NoUser_ReturnsFalse()
        {
            Assert.False(SessionUserHelper.IsAuthenticated(_session));
        }

        [Fact]
        public void HasRole_UserWithMatchingRole_ReturnsTrue()
        {
            SessionUserHelper.SetCurrentUser(_session, new UserDTO { Role = "Admin" });
            Assert.True(SessionUserHelper.HasRole(_session, "Admin"));
        }

        [Fact]
        public void HasRole_UserWithDifferentRole_ReturnsFalse()
        {
            SessionUserHelper.SetCurrentUser(_session, new UserDTO { Role = "User" });
            Assert.False(SessionUserHelper.HasRole(_session, "Admin"));
        }
    }
}