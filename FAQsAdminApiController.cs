{
   
    public class FAQsAdminApiController : ApiController
    {
        public IFAQsServices _faqsService = null;
        public IUserService _userService = null;
        public FAQsAdminApiController(IFAQsServices faqsService, IUserService userServiceInject)
        {
            _faqsService = faqsService; // private member of the instance
            _userService = userServiceInject;

        }

        [Route(), HttpPost]
        public HttpResponseMessage Insert(FAQsAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();

            string userId = _userService.GetCurrentUserId();

            response.Item = _faqsService.Insert(model, userId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage UpdateFAQs(FAQsUpdateRequest model, int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            string userId = _userService.GetCurrentUserId();

            _faqsService.Update(model, userId, id);

            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage Get(int id)
        {

            ItemResponse<FAQ> response = new ItemResponse<FAQ>();

            response.Item = _faqsService.Select(id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("cat/{id:int}"), HttpGet]
        public HttpResponseMessage GetCat(int id)
        {
            ItemResponse<FAQCategoryDomain> response = new ItemResponse<FAQCategoryDomain>();

            response.Item = _faqsService.SelectCat(id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpGet]
        public HttpResponseMessage GetAll()
        {

            ItemsResponse<FAQ> response = new ItemsResponse<FAQ>();

            response.Items = _faqsService.SelectAll();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            _faqsService.Delete(id);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}
