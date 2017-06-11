{

    public class BlogsAdminApiController : ApiController
    {
        IBlogService _blogService = null;
        private IUserService _userService = null;
        public BlogsAdminApiController(IBlogService blogService, IUserService userService)
        {
            _blogService = blogService; // private member of the instance
            _userService = userService;
        }

        [Route("search/{search}/{pageIndex:int}/{pageSize:int}"), HttpGet]

        public HttpResponseMessage GetSearchPage(string search, int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Blog>> response = new ItemResponse<PagedList<Blog>>();
            response.Item = _blogService.Search(search, pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /* For pagination */
        [Route("{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Blog>> response = new ItemResponse<PagedList<Blog>>();
            response.Item = _blogService.GetBlogsPaging(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{isPublished}/{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage GetByTab(bool isPublished, int pageIndex, int pageSize)
        {
            ItemResponse<PagedList<Blog>> response = new ItemResponse<PagedList<Blog>>();
            response.Item = _blogService.GetBlogTabsPaging(isPublished, pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // POST - CREATE
        [Route(), HttpPost]
        public HttpResponseMessage AddItem(BlogsAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();

            string userId = _userService.GetCurrentUserId();

            response.Item = _blogService.InsertBlog(model, userId); //InsertBlogs
            return Request.CreateResponse(HttpStatusCode.OK, response);


        }

        [Route("tags"), HttpPost]
        public HttpResponseMessage AddTag(BlogTagsAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();

            string userId = _userService.GetCurrentUserId();

            response.Item = _blogService.InsertTags(model, userId);
            return Request.CreateResponse(HttpStatusCode.OK, response);


        }

        // GET BY ID - READ
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage Get(int id)
        {
            ItemResponse<Blog> response = new ItemResponse<Blog>();
            response.Item = _blogService.Get(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("tags"), HttpGet]
        public HttpResponseMessage GetTags()
        {
            ItemsResponse<BlogsTagsDomain> response = new ItemsResponse<BlogsTagsDomain>();
            response.Items = _blogService.GetTags();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // GET ALL - READ
        [Route, HttpGet]
        public HttpResponseMessage Get()
        {
            ItemsResponse<Blog> response = new ItemsResponse<Blog>();
            response.Items = _blogService.Get();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        // GET ALL - READ
        [Route("published/{isPublished}"), HttpGet]
        public HttpResponseMessage GetPublished(bool isPublished)
        {
            ItemsResponse<Blog> response = new ItemsResponse<Blog>();
            response.Items = _blogService.GetPublished(isPublished);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // PUT - UPDATE
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage UpdateItem(BlogsUpdateRequest model, int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse response = new SuccessResponse();
            string currentUserId = _userService.GetCurrentUserId();
            _blogService.Update(model, currentUserId, id);
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        // DELETE
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            SuccessResponse response = new SuccessResponse();
            _blogService.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // GET ID BY SLUG - READ
        [Route("slug/{slug}/id"), HttpGet]
        public HttpResponseMessage GetBlogId(string slug)
        {
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = _blogService.GetBlogId(slug);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // GET BLOG BY SLUG - READ
        [Route("slug/{slug}"), HttpGet]
        public HttpResponseMessage GetBlog(string slug)
        {
            ItemResponse<Blog> response = new ItemResponse<Blog>();
            response.Item = _blogService.GetBlog(slug);

            if (response.Item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, response);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // GET ALL - READ
        [Route("slug/{slug}/related"), HttpGet]
        public HttpResponseMessage GetPublished()
        {
            ItemsResponse<Blog> response = new ItemsResponse<Blog>();
            response.Items = _blogService.GetPublished(true).PickRandom(3).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }


}
