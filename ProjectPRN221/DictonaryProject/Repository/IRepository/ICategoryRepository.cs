using DictonaryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject.Repository.IRepository
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategories();
        Category AddCategory(string categoryName);

        bool DeleteCategory(List<int> categoryIds);

        Category GetCategoryById(int categoryId);

        bool UpdateCategory(int categoryId, string categoryName);
        Category GetCategoryByName(string categoryName);

        List<Category> searchCategory(string keyword);


    }
}
