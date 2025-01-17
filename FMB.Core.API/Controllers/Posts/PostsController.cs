using FMB.Core.API.Controllers.BaseController;
using FMB.Core.API.Infrastructure.Services;
using FMB.Core.Data.Data;
using FMB.Core.Data.Models.Posts;
using FMB.Services.Posts;
using FMB.Services.Posts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FMB.Core.API.Controllers.Posts
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PostsController : BaseFMBController
    {
        private readonly IPostsService _postsService;
        public PostsController(IPostsService postsService, IConfiguration config, UserManager<AppUser> userManager) : base(userManager, config)
        {
            _postsService = postsService;
        }

        [HttpPost]
        public async Task<ActionResult<long>> CreatePost([FromBody] CreatePostRequest request)
        {
            if (request == null) { return new BadRequestObjectResult("empty request body"); }
            if (!TextValidator.IsPostTitleTextValid(request.Title)) { return new BadRequestObjectResult("invalid post title"); }
            if (!TextValidator.IsPostBodyTextValid(request.Body)) { return new BadRequestObjectResult("invalid post body"); }

            try
            {
                return await _postsService.CreatePostAsync(request.Title, request.Body, CurrentUser.Id);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }


        [HttpGet]
        public async Task<Post> GetPost([FromBody] GetPostRequest request) // TODO return DTO with rating, comments, tags
        {
            return await _postsService.GetPostAsync(request.Id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost(long postId) // TODO only for moderators, consider archivation, add logging
        {
            await _postsService.DeletePostAsync(postId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostRequest request)
        {
            // TODO check author
            // TODO check body and title
            await _postsService.UpdatePostAsync(request.Id, request.NewBody, request.NewTitle);
            return Ok();
        }
    }
}