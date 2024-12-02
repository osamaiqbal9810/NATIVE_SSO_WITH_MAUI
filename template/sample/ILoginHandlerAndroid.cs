
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatWithDocsMobileApp
{
    public interface ILoginHandlerAndroid
    {
        event Action<GoogleUserInfo?> onGoogleLoginCompleted;
        void handleLoginAsync();
    }
}
