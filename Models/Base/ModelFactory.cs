using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreApi.Models.Base
{
    public class ModelFactory
    {
        public static List<IdentifiedServiceModel> ModelList
        {
            get
            {
                List<IdentifiedServiceModel> models = new List<IdentifiedServiceModel>();
                
                models.Add(new CompanyModel()); 
                models.Add(new ProductModel()); 

                return models;
            }
        }

        public static Type GetModelType(string modelName)
        { 
            IdentifiedServiceModel model = ModelList.FirstOrDefault(i => i.GetType().Name.ToLower() == modelName.ToLower() || i.GetType().Name.ToLower() == modelName.ToLower() + "model");

            if (model != null) 
                return model.GetType(); 

            return null;
        }
    }
}