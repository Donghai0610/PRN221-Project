using DictonaryProject.DataAccess;
using DictonaryProject.Models;
using DictonaryProject.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject.Repository
{
   public class CategoryRepository : ICategoryRepository
    {
        public Category AddCategory(string categoryName)
        {
           return DictionaryDAO.Instance.AddCategory(categoryName);
        }

        public bool DeleteCategory(List<int> categoryIds)
        {
            return CategoryDAO.Instance.DeleteCategory(categoryIds);
        }

        public List<Category> GetAllCategories()
        {
            return CategoryDAO.Instance.GetAllCategories();
        }

        public Category GetCategoryById(int categoryId)
        {
            return CategoryDAO.Instance.GetCategoryById(categoryId);
        }

        public Category GetCategoryByName(string categoryName)
        {
           return CategoryDAO.Instance.GetCategoryByName(categoryName);
        }

        public List<Category> searchCategory(string keyword)
        {
            return CategoryDAO.Instance.searchCategory(keyword);
        }

        public bool UpdateCategory(int categoryId, string categoryName)
        {
            return CategoryDAO.Instance.UpdateCategory(categoryId, categoryName);
        }
    }
}
