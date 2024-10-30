using DictonaryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject.DataAccess
{
    public class CategoryDAO
    {
        private static CategoryDAO _instance = null;
        private static readonly object instancelock = new object();

        public CategoryDAO() { }
        public static CategoryDAO Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryDAO();
                    }
                    return _instance;
                }
            }
        }



        public List<Category> GetAllCategories()
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                return context.Categories
                              .Select(c => new Category
                              {
                                  CategoryId = c.CategoryId,
                                  CategoryName = c.CategoryName,
                              })
                              .ToList();
            }
        }


        public bool DeleteCategory(List<int> categoryIds)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                var categoriesToDelete = context.Categories.Where(c => categoryIds.Contains(c.CategoryId)).ToList();

                // Kiểm tra nếu có danh mục nào được tìm thấy
                if (categoriesToDelete.Any())
                {
                    context.Categories.RemoveRange(categoriesToDelete); 
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }



        public Category GetCategoryById(int categoryId)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                return context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            }
        }
        public Category GetCategoryByName(string categoryName)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                return context.Categories.FirstOrDefault(c => c.CategoryName.Equals(categoryName));
            }
        }

        public bool UpdateCategory(int categoryId, string categoryName)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                var category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                if (category != null)
                {
                    category.CategoryName = categoryName;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public List<Category> searchCategory(string keyword)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                return context.Categories.Where(c => c.CategoryName.Contains(keyword)).Select( c => new Category
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                }).ToList();
            }
        }
    }
}
