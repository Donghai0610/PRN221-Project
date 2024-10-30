using DictonaryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DictonaryProject.DataAccess
{
    internal class DictionaryDAO
    {
        private static DictionaryDAO _instance = null;
        private static readonly object instancelock = new object();

        public DictionaryDAO() { }
        public static DictionaryDAO Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (_instance == null)
                    {
                        _instance = new DictionaryDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<object> GetAllDictionary()
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                // Truy vấn các từ đã được phê duyệt
                var dictionary = context.Dictionaries
                    .Where(d => d.IsApproved == true) // Chỉ lấy các từ được phê duyệt
                    .Select(d => new
                    {
                        WordID = d.WordId,
                        EnglishWord = d.EnglishWord,
                        TypeOfWord = d.TypeOfWord,
                        Pronunciation = d.Pronunciation,
                        Categories = d.Categories.Select(c => c.CategoryName).ToList(),
                        Meanings = d.Meanings.Select(m => new
                        {
                            MeaningEnglish = m.EnglishMeaning,
                            MeaningVietnamese = m.VietnameseMeaning,
                            Example = m.ExampleSentence
                        }).ToList()
                    })
                    .ToList();

                // Tạo danh sách đối tượng trả về
                var result = dictionary.Select(d => new
                {
                    WordId = d.WordID,
                    EnglishWord = d.EnglishWord,
                    TypeOfWord = d.TypeOfWord,
                    Pronunciation = d.Pronunciation,
                    CategoryName = string.Join(", ", d.Categories), // Gộp tên danh mục thành chuỗi
                    MeaningEnglish = d.Meanings.Select(m => m.MeaningEnglish).FirstOrDefault(),
                    MeaningVietnamese = d.Meanings.Select(m => m.MeaningVietnamese).FirstOrDefault(),
                    Example = d.Meanings.Select(m => m.Example).FirstOrDefault()
                })
                .ToList<object>();

                return result;
            }
        }

        //lay all category
        public List<Category> GetAllCategories() => new PersonalDictionaryDBContext().Categories.ToList();

        public List<object> SearchByWordAndCategory(string keyword, string categoryName)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                var query = context.Dictionaries.AsQueryable();

                // Lọc theo từ khóa nếu có
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(d => d.EnglishWord.Contains(keyword));
                }

                // Lọc theo danh mục nếu có
                if (!string.IsNullOrEmpty(categoryName))
                {
                    query = query.Where(d => d.Categories.Any(c => c.CategoryName.Contains(categoryName)));
                }

                // Lấy thông tin cần thiết từ kết quả tìm kiếm
                var results = query
                    .Where(d => d.IsApproved == true) // Chỉ lấy các từ đã được phê duyệt
                    .Select(d => new
                    {
                        WordId = d.WordId,
                        EnglishWord = d.EnglishWord,
                        TypeOfWord = d.TypeOfWord,
                        Pronunciation = d.Pronunciation,
                        CategoryName = string.Join(", ", d.Categories.Select(c => c.CategoryName)), // Gộp tên danh mục thành chuỗi
                        MeaningEnglish = d.Meanings.Select(m => m.EnglishMeaning).FirstOrDefault(),
                        MeaningVietnamese = d.Meanings.Select(m => m.VietnameseMeaning).FirstOrDefault(),
                        ExampleSentence = d.Meanings.Select(m => m.ExampleSentence).FirstOrDefault()
                    })
                    .ToList<object>();

                return results;
            }
        }
        public bool ValidateWordExistence(string englishWord, string englishMeaning, string vietnameseMeaning)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                return context.Dictionaries
                    .Any(d => d.EnglishWord == englishWord &&
                              d.Meanings.Any(m => m.EnglishMeaning.Equals(englishMeaning) &&
                                                  m.VietnameseMeaning.Equals(vietnameseMeaning)));
            }
        }


        public bool AddNewWord(string englishWord, string typeOfWord, List<string> categoryNames, string pronunciation,
                       int createdByUserId, string englishMeaning, string vietnameseMeaning,
                       string exampleSentence, bool isUser)

        {

            if (ValidateWordExistence(englishWord, englishMeaning, vietnameseMeaning))
            {
                MessageBox.Show("Từ đã tồn tại trong từ điển", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                try
                {
                    // Tạo đối tượng từ mới
                    Dictionary newWord = new Dictionary
                    {
                        EnglishWord = englishWord,
                        TypeOfWord = typeOfWord,
                        Pronunciation = pronunciation,
                        CreatedBy = createdByUserId,
                        CreatedDate = DateTime.Now,
                        IsApproved = !isUser // Nếu là user thì IsApproved = false
                    };

                    foreach (var categoryName in categoryNames)
                    {
                        Category category;
                        category = context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);


                        if (category != null && !newWord.Categories.Contains(category))
                        {
                            newWord.Categories.Add(category);
                        }
                    }

                    // Thêm nghĩa cho từ mới
                    Meaning newMeaning = new Meaning
                    {
                        EnglishMeaning = englishMeaning,
                        VietnameseMeaning = vietnameseMeaning,
                        ExampleSentence = exampleSentence,
                        Word = newWord
                    };

                    context.Dictionaries.Add(newWord);
                    context.Meanings.Add(newMeaning);

                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã có lỗi xảy ra: {ex.Message}\n{ex.StackTrace}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }


        public Category AddCategory(string categoryName)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                // Kiểm tra xem danh mục đã tồn tại chưa
                var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);

                if (existingCategory == null)
                {
                    // Nếu chưa tồn tại, thêm danh mục mới
                    var newCategory = new Category { CategoryName = categoryName };
                    context.Categories.Add(newCategory);
                    context.SaveChanges(); // Lưu thay đổi để có CategoryID

                    return newCategory;
                }

                // Nếu đã tồn tại, trả về danh mục hiện có
                return existingCategory;
            }
        }






    }
}
