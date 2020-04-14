using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CommentAndReview.Startup))]
namespace CommentAndReview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
