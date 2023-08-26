using System.Collections.Generic;
using CoreApi.Models.Base;

namespace CoreApi.HelperCodes.Model
{
    public class ModelCreator
    {
         public static IdentifiedServiceModel GetModelFromName(string modelName)
        {
            IdentifiedServiceModel obj = null;
            List<IdentifiedServiceModel> allModels = ModelFactory.ModelList;

            foreach (IdentifiedServiceModel tp in allModels)
            {
                if (Helper.ModelCheckName(tp.GetType().Name, modelName)) 
                {
                    obj = tp;
                    break;
                }
            }

            return obj;
        }
    }
}