
    public class FAQsServices : BaseService, IFAQsServices
    {
        public int Insert(FAQsAddRequest model, string userId)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.FAQs_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {


                   SqlParameter ids = new SqlParameter("@CatList", System.Data.SqlDbType.Structured);

                   if (model.Categories != null && model.Categories.Any())
                   {
                       ids.Value = new object.Data.IntIdTable(model.Categories);
                   }

                   paramCollection.Add(ids);



                   paramCollection.AddWithValue("@Question", model.Question); ;
                   paramCollection.AddWithValue("@DisplayOrder", model.DisplayOrder);
                   paramCollection.AddWithValue("@CreatedDate", model.CreatedDate);
                   paramCollection.AddWithValue("@CreatedBy", userId);
                   paramCollection.AddWithValue("@ModifiedDate", model.ModifiedDate);
                   paramCollection.AddWithValue("@Answer", model.Answer);

                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out id);
               }
               );


            return id;
        }


        public void Update(FAQsUpdateRequest model, string userId, int Id)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.FAQs_Update"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

                   SqlParameter ids = new SqlParameter("@CatList", System.Data.SqlDbType.Structured);

                   if (model.Categories != null && model.Categories.Any())
                   {
                       ids.Value = new object.Data.IntIdTable(model.Categories);
                   }

                   paramCollection.Add(ids);

                   paramCollection.AddWithValue("@Id", Id);
                   paramCollection.AddWithValue("@Question", model.Question); ;
                   paramCollection.AddWithValue("@DisplayOrder", model.DisplayOrder);
                   paramCollection.AddWithValue("@ModifiedBy", userId);
                   paramCollection.AddWithValue("@Answer", model.Answer);


               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out id);
               }
               );

        }


        public void Delete(int Id)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.FAQs_Delete"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

                   paramCollection.AddWithValue("@Id", Id);


               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out id);
               }
               );

        }


        public FAQ Select(int id)
        {
            FAQ obj = null;
            List<FAQCategoryDomain> faqList = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.FAQs_SelectById_V2"
               , inputParamMapper: delegate (SqlParameterCollection parmas)
               {
                   parmas.AddWithValue("@Id", id);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   if (set == 0)
                   {
                       obj = new FAQ();
                       MapFaq(reader, obj);
                   }
                   else if (set == 1)
                   {
                       FAQCategoryDomain f = MapFAQCategory(reader);

                       if (faqList == null)
                       {
                           faqList = new List<FAQCategoryDomain>();
                       }

                       faqList.Add(f);
                   }
               });
            if (obj != null)
            {
                obj.Categories = faqList;
            }

            return obj;

        }

        public FAQCategoryDomain SelectCat(int id)
        {
            FAQCategoryDomain obj = null;
            List<FAQ> faqList = null;

            DataProvider.ExecuteCmd(GetConnection, "FAQs_SelectCatById_V2"
               , inputParamMapper: delegate (SqlParameterCollection parmas)
               {
                   parmas.AddWithValue("@Id", id);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   if (set == 0)
                   {
                       obj = new FAQCategoryDomain();
                       MapFAQCat(reader, obj);
                   }
                   else if (set == 1)
                   {
                       FAQ f = MapFAQ(reader);


                       if (faqList == null)
                       {
                           faqList = new List<FAQ>();
                       }

                       faqList.Add(f);
                   }
               });
            if (obj != null)
            {
                obj.FAQs = faqList;
            }


            return obj;

        }

        public List<FAQ> SelectAll()
        {
            List<FAQ> faqList = null;

            List<FAQCategoryDomain> list = null;

            //FaqId, is a list of CatId
            Dictionary<int, List<int>> dict = null;


            DataProvider.ExecuteCmd(GetConnection, "dbo.FAQs_SelectAll_V2"
               , inputParamMapper: null
               , map: delegate (IDataReader reader, short set)
               {

                   if (set == 0)
                   {
                       FAQ question = new FAQ();

                       MapFaq(reader, question);
                       if (faqList == null)
                       {
                           faqList = new List<FAQ>();
                       }
                       faqList.Add(question);
                   }

                   else if (set == 1)
                   {
                       FAQCategoryDomain f = MapFAQCategory(reader);

                       if (list == null)
                       {
                           list = new List<FAQCategoryDomain>();
                       }
                       list.Add(f);
                   }

                   else if (set == 2)
                   {
                       int catId = reader.GetSafeInt32(0);
                       int faqId = reader.GetSafeInt32(1);

                       if (dict == null)
                       {
                           dict = new Dictionary<int, List<int>>();
                       }
                       if (!dict.ContainsKey(faqId))
                       {
                           dict.Add(faqId, new List<int>());
                       }
                       List<int> myList = dict[faqId];

                       myList.Add(catId);
                   }
               });

            if (faqList != null && dict != null)
            {
                foreach (FAQ item in faqList)
                {
                    if (!dict.ContainsKey(item.Id))
                    {
                        continue;
                    }

                    List<int> catIds = dict[item.Id];
                    List<FAQCategoryDomain> cats = new List<FAQCategoryDomain>();
                    foreach (int catId in catIds)
                    {
                        FAQCategoryDomain d = list.FirstOrDefault(c => c.id == catId);


                        cats.Add(d);
                    }

                    item.Categories = cats;
                }

            }

            return faqList;
        }

        private void MapFaq(IDataReader reader, FAQ obj)
        {
            int number = 0;
            obj.Id = reader.GetSafeInt32(number++);
            obj.Question = reader.GetSafeString(number++);
            obj.Answer = reader.GetSafeString(number++);
            obj.DisplayOrder = reader.GetSafeInt32(number++);
            obj.CreatedBy = reader.GetSafeString(number++);
            obj.ModifiedBy = reader.GetSafeString(number++);
            obj.ModifiedDate = reader.GetSafeDateTime(number++);
            obj.CreatedDate = reader.GetSafeDateTime(number++);
        }



        private static FAQCategoryDomain MapFAQCategory(IDataReader reader)
        {
            FAQCategoryDomain f = new FAQCategoryDomain();
            int startingIndex = 0;

            f.id = reader.GetSafeInt32(startingIndex++);
            f.title = reader.GetSafeString(startingIndex++);
            f.description = reader.GetSafeString(startingIndex++);
            f.displayOrder = reader.GetSafeInt32(startingIndex++);
            f.createdBy = reader.GetSafeString(startingIndex++);
            f.createdDate = reader.GetSafeDateTime(startingIndex++);
            f.modifiedBy = reader.GetSafeString(startingIndex++);
            f.modifiedDate = reader.GetSafeDateTime(startingIndex++);
            f.ImageURL = reader.GetSafeString(startingIndex++);

            return f;
        }
        private void MapFAQCat(IDataReader reader, FAQCategoryDomain obj)
        {

            int startingIndex = 0;

            obj.id = reader.GetSafeInt32(startingIndex++);
            obj.title = reader.GetSafeString(startingIndex++);
            obj.description = reader.GetSafeString(startingIndex++);
            obj.displayOrder = reader.GetSafeInt32(startingIndex++);
            obj.createdBy = reader.GetSafeString(startingIndex++);
            obj.createdDate = reader.GetSafeDateTime(startingIndex++);
            obj.modifiedBy = reader.GetSafeString(startingIndex++);
            obj.modifiedDate = reader.GetSafeDateTime(startingIndex++);
            obj.ImageURL = reader.GetSafeString(startingIndex++);



        }

        private static FAQ MapFAQ(IDataReader reader)
        {
            FAQ f = new FAQ();
            int number = 0;
            f.Id = reader.GetSafeInt32(number++);
            f.Question = reader.GetSafeString(number++);
            f.Answer = reader.GetSafeString(number++);
            f.DisplayOrder = reader.GetSafeInt32(number++);
            f.CreatedBy = reader.GetSafeString(number++);
            f.ModifiedBy = reader.GetSafeString(number++);
            f.ModifiedDate = reader.GetSafeDateTime(number++);
            f.CreatedDate = reader.GetSafeDateTime(number++);
            return f;
        }
    }


