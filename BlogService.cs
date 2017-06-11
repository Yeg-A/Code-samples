
    public class BlogService : BaseService, IBlogService
    {
        public PagedList<Blog> Search(string searchText, int pageIndex, int pageSize)
        {
            PagedList<Blog> pagedList = null;
            int totalCount = 0;

            List<Blog> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_SearchByTitle"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@SearchText", searchText);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
               
                }
                 , map: delegate (IDataReader reader, short set)
                 {
                     Blog b;
                     MapWithTotalCount(reader, out b);

                     if (list == null)
                     {
                         list = new List<Blog>();
                     }
                     list.Add(b);
                     if (totalCount == 0)
                         totalCount = b.TotalCount;
                 });

            if (list != null)
            {
                pagedList = new PagedList<Blog>(list, pageIndex, pageSize, totalCount);
            }


            return pagedList;
        }
        // Get All Comments with pagination
        public PagedList<Blog> GetBlogsPaging(int pageIndex, int pageSize)
        {
            PagedList<Blog> pagedList = null;
            int totalCount = 0;

            List<Blog> list = null;

            DataProvider.ExecuteCmd(GetConnection
                , "dbo.Blog_SelectPaginate"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                }
                , map: delegate (IDataReader reader, short set)
                {

                    Blog b;
                    MapWithTotalCount(reader, out b);

                    if (list == null)
                    {
                        list = new List<Blog>();
                    }
                    list.Add(b);
                    if (totalCount == 0)
                        totalCount = b.TotalCount;
                });

            if (list != null)
            {
                pagedList = new PagedList<Blog>(list, pageIndex, pageSize, totalCount);
            }


            return pagedList;
        }


        public PagedList<Blog> GetBlogTabsPaging(bool isPublished, int pageIndex, int pageSize)
        {
            PagedList<Blog> pagedList = null;
            int totalCount = 0;

            List<Blog> list = null;

            DataProvider.ExecuteCmd(GetConnection
                , "dbo.Blogs_TabPagination"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                    paramCollection.AddWithValue("@isPublished", isPublished);

                }
                , map: delegate (IDataReader reader, short set)
                {

                    Blog b;
                    MapWithTotalCount(reader, out b);

                    if (list == null)
                    {
                        list = new List<Blog>();
                    }
                    list.Add(b);
                    if (totalCount == 0)
                        totalCount = b.TotalCount;
                });

            if (list != null)
            {
                pagedList = new PagedList<Blog>(list,pageIndex, pageSize, totalCount);
            }


            return pagedList;
        }
        // CREATE (INSERT)
        public int InsertBlog(BlogsAddRequest model, string userId) // Dependency Injection: remove static
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Blogs_Insert_V2", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {


                SqlParameter ids = new SqlParameter("@TagList", System.Data.SqlDbType.Structured);

                if (model.Tags != null && model.Tags.Any())
                {
                    ids.Value = new object.Data.IntIdTable(model.Tags);
                }

                paramCollection.Add(ids);

                paramCollection.AddWithValue("@Title", model.Title);
                paramCollection.AddWithValue("@Subject", model.Subject);
                paramCollection.AddWithValue("@PublishDate", model.PublishDate);
                paramCollection.AddWithValue("@Content", model.Content);
                paramCollection.AddWithValue("@isPublished", model.isPublished);
                paramCollection.AddWithValue("@DateCreated", model.DateCreated);
                paramCollection.AddWithValue("@DateModified", model.DateModified);


                paramCollection.AddWithValue("@Slug", model.Slug);
                paramCollection.AddWithValue("@ImageUrl", model.ImageUrl);


                SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                p.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(p);
            }
            , returnParameters: delegate (SqlParameterCollection param)
            {
                int.TryParse(param["@Id"].Value.ToString(), out id);
            }
            );
            return id;
        }

        public int InsertTags(BlogTagsAddRequest model, string userId)
        {
            int Id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "blogtags_insert"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)

                {
                    paramCollection.AddWithValue("@Tags", model.text);
                    paramCollection.AddWithValue("@DateAdded", null);
                    paramCollection.AddWithValue("@DateModified", null);

                    SqlParameter p = new SqlParameter("@ID", SqlDbType.Int);
                    p.Direction = ParameterDirection.Output;

                    paramCollection.Add(p);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    int.TryParse(param["@ID"].Value.ToString(), out Id);
                }
                );

            return Id;
        }

        // READ (GET BY ID)
        public Blog Get(int id) // pass in datatype and id to get specific Blog; // Dependency Injection: remove static
        {
            Blog p = null;
            List<BlogsTagsDomain> blogList = null;

            DataProvider.ExecuteCmd(GetConnection, "Blogs_SelectById_V2", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }
            , map: delegate (IDataReader reader, short set)
            {
                if (set == 0)
                {
                    p = new Blog();
                    mapBlog(reader, p);
                }
                else if (set == 1)
                {
                    BlogsTagsDomain f = mapTag(reader);

                    if (blogList == null)
                    {
                        blogList = new List<BlogsTagsDomain>();
                    }

                    blogList.Add(f);
                }
            });
            if (p != null)
            {
                p.Tags = blogList;
            }

            return p;

        }


        // READ (GET ALL)
        public List<Blog> Get() // Dependency Injection: remove static
        {
            List<Blog> blogList = null;

            List<BlogsTagsDomain> list = null;
            //BlogId, is a list of TagId
            Dictionary<int, List<int>> dict = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_SelectAll_V2", inputParamMapper: null, map: delegate (IDataReader reader, short set)

            {

                if (set == 0)
                {
                    Blog topic = new Blog();

                    mapBlog(reader, topic);
                    if (blogList == null)
                    {
                        blogList = new List<Blog>();
                    }
                    blogList.Add(topic);
                }

                else if (set == 1)
                {
                    BlogsTagsDomain f = mapTag(reader);

                    if (list == null)
                    {
                        list = new List<BlogsTagsDomain>();
                    }
                    list.Add(f);
                }

                else if (set == 2)
                {
                    int TagId = reader.GetSafeInt32(0);
                    int BlogId = reader.GetSafeInt32(1);

                    if (dict == null)
                    {
                        dict = new Dictionary<int, List<int>>();
                    }
                    if (!dict.ContainsKey(BlogId))
                    {
                        dict.Add(BlogId, new List<int>());
                    }
                    List<int> myList = dict[BlogId];

                    myList.Add(TagId);
                }
            });

            if (blogList != null && dict != null)
            {
                foreach (Blog item in blogList)
                {
                    if (!dict.ContainsKey(item.Id))
                    {
                        continue;
                    }

                    List<int> TagIds = dict[item.Id];
                    List<BlogsTagsDomain> tags = new List<BlogsTagsDomain>();
                    foreach (int TagId in TagIds)
                    {
                        BlogsTagsDomain d = list.FirstOrDefault(c => c.id == TagId);


                        tags.Add(d);
                    }

                    item.Tags = tags;
                }

            }

            return blogList;
        }


        // READ (GET Blogs by Publish status)
        public List<Blog> GetPublished(bool isPublished) // Dependency Injection: remove static
        {
            List<Blog> blogList = null;

            List<BlogsTagsDomain> list = null;
            //BlogId, is a list of TagId
            Dictionary<int, List<int>> dict = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_Published", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@isPublished", isPublished);
            }
          , map: delegate (IDataReader reader, short set)

          {

              if (set == 0)
              {
                  Blog topic = new Blog();

                  mapBlog(reader, topic);
                  if (blogList == null)
                  {
                      blogList = new List<Blog>();
                  }
                  blogList.Add(topic);
              }

              else if (set == 1)
              {
                  BlogsTagsDomain f = mapTag(reader);

                  if (list == null)
                  {
                      list = new List<BlogsTagsDomain>();
                  }
                  list.Add(f);
              }

              else if (set == 2)
              {
                  int TagId = reader.GetSafeInt32(0);
                  int BlogId = reader.GetSafeInt32(1);

                  if (dict == null)
                  {
                      dict = new Dictionary<int, List<int>>();
                  }
                  if (!dict.ContainsKey(BlogId))
                  {
                      dict.Add(BlogId, new List<int>());
                  }
                  List<int> myList = dict[BlogId];

                  myList.Add(TagId);
              }
          });

            if (blogList != null && dict != null)
            {
                foreach (Blog item in blogList)
                {
                    if (!dict.ContainsKey(item.Id))
                    {
                        continue;
                    }

                    List<int> TagIds = dict[item.Id];
                    List<BlogsTagsDomain> tags = new List<BlogsTagsDomain>();
                    foreach (int TagId in TagIds)
                    {
                        BlogsTagsDomain d = list.FirstOrDefault(c => c.id == TagId);


                        tags.Add(d);
                    }

                    item.Tags = tags;
                }

            }

            return blogList;
        }

        // UPDATE
        public void Update(BlogsUpdateRequest model, string userId, int id) // Dependency Injection: remove static
        {

            int Id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Blogs_Update_V2", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                SqlParameter ids = new SqlParameter("@TagList", System.Data.SqlDbType.Structured);

                if (model.Tags != null && model.Tags.Any())
                {
                    ids.Value = new object.Data.IntIdTable(model.Tags);
                }

                paramCollection.Add(ids);
                paramCollection.AddWithValue("@Title", model.Title);
                paramCollection.AddWithValue("@Subject", model.Subject);
                paramCollection.AddWithValue("@PublishDate", model.PublishDate); 
                paramCollection.AddWithValue("@Content", model.Content);
                paramCollection.AddWithValue("@isPublished", model.isPublished);
                paramCollection.AddWithValue("@Slug", model.Slug);


                paramCollection.AddWithValue("@Id", id); // need this to update the specified property in Blogs; also need to disable this in the form
            }, returnParameters: delegate (SqlParameterCollection param)
            {
                int.TryParse(param["@Id"].Value.ToString(), out Id);

            });
        }

        // DELETE
        public void Delete(int id) // pass in datatype and id to target the specific item; // Dependency Injection: remove static
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Blogs_Delete", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            });
        }

        // SELECT ALL 
        public List<BlogsTagsDomain> GetTags()
        {
            List<BlogsTagsDomain> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.BlogTags_SelectAll"
                , inputParamMapper: null
                , map: delegate (IDataReader reader, short set)
                {
                    BlogsTagsDomain t = mapTag(reader);

                    if (list == null)
                    {
                        list = new List<BlogsTagsDomain>();
                    }

                    list.Add(t);
                }
                );

            return list;
        }

        public int GetBlogId(string slug)
        {
            int blog = 0;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_SelectIdBySlug", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Slug", slug);
            }
            , map: delegate (IDataReader reader, short set)
            {
                blog = new int();
                int startingIndex = 0;

                blog = reader.GetSafeInt32(startingIndex++);

                if (blog == 0)
                {
                    blog = new int();
                }
            }
            );
            return blog;
        }

        // GET BLOG BY SLUG - READ
        public Blog GetBlog(string slug)
        {

            Blog p = null;
            List<BlogsTagsDomain> blogList = null;


            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_SelectBlogBySlug", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Slug", slug);
            }
            , map: delegate (IDataReader reader, short set)
            {
                if (set == 0)
                {
                    p = new Blog();
                    mapBlog(reader, p);
                }
                else if (set == 1)
                {
                    BlogsTagsDomain f = mapTag(reader);

                    if (blogList == null)
                    {
                        blogList = new List<BlogsTagsDomain>();
                    }

                    blogList.Add(f);
                }
            });
            if (p != null)
            {
                p.Tags = blogList;
            }

            return p;

        }

        // GETS BLOGS BY TAG NAME
        public PagedList<Blog> GetByTagName(string tagName, int pageIndex, int pageSize)
        {
            PagedList<Blog> pagedList = null;
            List<Blog> list = null;
            int totalCount = 0;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_SelectByTagName"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@TagName", tagName);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                }
                , map: delegate (IDataReader reader, short set)
                {
                    Blog b;
                    MapWithTotalCount(reader, out b);

                    if (list == null)
                    {
                        list = new List<Blog>();
                    }
                    list.Add(b);
                    if (totalCount == 0)
                    {
                        totalCount = b.TotalCount;
                    }
                }
            );

            if (list != null)
            {
                pagedList = new PagedList<Blog>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }

        private void mapBlog(IDataReader reader, Blog b)
        {
            int startingIndex = 0;

            b.Id = reader.GetSafeInt32(startingIndex++);
            b.Title = reader.GetSafeString(startingIndex++);
            b.Subject = reader.GetSafeString(startingIndex++);
            b.Author = reader.GetSafeString(startingIndex++);
            b.Date = reader.GetSafeDateTime(startingIndex++);
            b.Content = reader.GetSafeString(startingIndex++);
            b.isPublished = reader.GetSafeBool(startingIndex++);
            b.DateCreated = reader.GetSafeDateTime(startingIndex++);
            b.DateModified = reader.GetSafeDateTime(startingIndex++);
            b.Slug = reader.GetSafeString(startingIndex++);
            b.ImageUrl = reader.GetSafeString(startingIndex++);

        }

        private Blog MapWithTotalCount(IDataReader reader, out Blog b)
        {
            b = new Blog();
            int startingIndex = 0;

            b.Id = reader.GetSafeInt32(startingIndex++);
            b.Title = reader.GetSafeString(startingIndex++);
            b.Subject = reader.GetSafeString(startingIndex++);
            b.Author = reader.GetSafeString(startingIndex++);
            b.Date = reader.GetSafeDateTime(startingIndex++);
            b.Content = reader.GetSafeString(startingIndex++);
            b.isPublished = reader.GetSafeBool(startingIndex++);
            b.DateCreated = reader.GetSafeDateTime(startingIndex++);
            b.DateModified = reader.GetSafeDateTime(startingIndex++);
            b.Slug = reader.GetSafeString(startingIndex++);
            b.ImageUrl = reader.GetSafeString(startingIndex++);
            b.TotalCount = reader.GetSafeInt32(startingIndex++);


            return b;
        }

        private static BlogsTagsDomain mapTag(IDataReader reader)
        {
            BlogsTagsDomain t = new BlogsTagsDomain();
            int startingIndex = 0;

            t.id = reader.GetSafeInt32(startingIndex++);
            t.text = reader.GetSafeString(startingIndex++);

            t.dateAdded = reader.GetSafeDateTime(startingIndex++);

            t.dateModified = reader.GetSafeDateTime(startingIndex++);


            return t;
        }

        public PagedList<Blog> GetAuthorsPaging(string author, int pageIndex, int pageSize)
        {
            PagedList<Blog> pagedList = null;
            int totalCount = 0;

            List<Blog> list = null;

            DataProvider.ExecuteCmd(GetConnection, "Blogs_SelectByAuthorPaging"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Author", author);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                    
                }
                 , map: delegate (IDataReader reader, short set)
                 {
                     Blog b;
                     MapWithTotalCount(reader, out b);

                     if (list == null)
                     {
                         list = new List<Blog>();
                     }
                     list.Add(b);
                     if (totalCount == 0)
                         totalCount = b.TotalCount;
                 });
            if (list != null)
            {
                pagedList = new PagedList<Blog>(list, 0, pageSize, totalCount);
            }

            return pagedList;
        }

        public PagedList<Blog> SearchAuthor(string author, string searchText, int pageIndex, int pageSize)
        {
            PagedList<Blog> pagedList = null;
            int totalCount = 0;

            List<Blog> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Blogs_SearchAuthorByTitles"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Author", author);
                    paramCollection.AddWithValue("@SearchText", searchText);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);

                }
                 , map: delegate (IDataReader reader, short set)
                 {
                     Blog b;
                     MapWithTotalCount(reader, out b);

                     if (list == null)
                     {
                         list = new List<Blog>();
                     }
                     list.Add(b);
                     if (totalCount == 0)
                         totalCount = b.TotalCount;
                 });

            if (list != null)
            {
                pagedList = new PagedList<Blog>(list, pageIndex, pageSize, totalCount);
            }


            return pagedList;
        }
    }


