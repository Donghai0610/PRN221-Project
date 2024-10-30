using DictonaryProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DictonaryProject.DataAccess
{
    public class DictionaryManagementDAO
    {
        public DictionaryManagementDAO() { }
        private static DictionaryManagementDAO _instance = null;
        private static readonly object instancelock = new object();
        public static DictionaryManagementDAO Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (_instance == null)
                    {
                        _instance = new DictionaryManagementDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<object> GetAllDictionaryManagement()
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                var dictionary = context.Dictionaries
                    .OrderBy(d => d.IsApproved) 
                    .Select(d => new
                    {
                        WordID = d.WordId,
                        EnglishWord = d.EnglishWord,
                        TypeOfWord = d.TypeOfWord,
                        Pronunciation = d.Pronunciation,
                        IsApproved = d.IsApproved,
                        CreatedBy = d.CreatedByNavigation.Username, 
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
                    CreatedBy = d.CreatedBy, // Tên người tạo
                    CategoryName = string.Join(", ", d.Categories), // Gộp tên danh mục thành chuỗi
                    MeaningEnglish = d.Meanings.Select(m => m.MeaningEnglish).FirstOrDefault(),
                    MeaningVietnamese = d.Meanings.Select(m => m.MeaningVietnamese).FirstOrDefault(),
                    Example = d.Meanings.Select(m => m.Example).FirstOrDefault(),
                    IsApproved = d.IsApproved == true ? "Đã được duyệt" : "Đợi duyệt",
                })
                .ToList<object>();

                return result;
            }
        }


        public List<object> SearchByWordAndCategory(string keyword, string categoryName)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                var query = context.Dictionaries.AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(d => d.EnglishWord.Contains(keyword));
                }

                if (!string.IsNullOrEmpty(categoryName))
                {
                    query = query.Where(d => d.Categories.Any(c => c.CategoryName.Contains(categoryName)));
                }

                var dictionary = query
                    .OrderBy(d => d.IsApproved)
                    .Select(d => new
                    {
                        WordID = d.WordId,
                        EnglishWord = d.EnglishWord,
                        TypeOfWord = d.TypeOfWord,
                        Pronunciation = d.Pronunciation,
                        IsApproved = d.IsApproved,
                        CreatedBy = d.CreatedByNavigation.Username,
                        Categories = d.Categories.Select(c => c.CategoryName).ToList(),
                        Meanings = d.Meanings.Select(m => new
                        {
                            MeaningEnglish = m.EnglishMeaning,
                            MeaningVietnamese = m.VietnameseMeaning,
                            Example = m.ExampleSentence
                        }).ToList()
                    })
                    
                    .ToList();

                var result = dictionary.Select(d => new
                {
                    WordId = d.WordID,
                    EnglishWord = d.EnglishWord,
                    TypeOfWord = d.TypeOfWord,
                    Pronunciation = d.Pronunciation,
                    CreatedBy = d.CreatedBy, // Tên người tạo
                    CategoryName = string.Join(", ", d.Categories), // Gộp tên danh mục thành chuỗi
                    MeaningEnglish = d.Meanings.Select(m => m.MeaningEnglish).FirstOrDefault(),
                    MeaningVietnamese = d.Meanings.Select(m => m.MeaningVietnamese).FirstOrDefault(),
                    Example = d.Meanings.Select(m => m.Example).FirstOrDefault(),
                    IsApproved = d.IsApproved == true ? "Đã được duyệt" : "Đợi duyệt"
                })
                .ToList<object>();

                return result;
            }
        }


        public int GetUnapprovedWordCount()
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                // Đếm số từ chưa được phê duyệt
                int unapprovedWordCount = context.Dictionaries
                    .Count(d => d.IsApproved == false);

                return unapprovedWordCount;
            }
        }


        public bool ApproveWord(int wordId)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                try
                {
                    var word = context.Dictionaries.FirstOrDefault(d => d.WordId == wordId);

                    if (word == null)
                    {
                        return false;
                    }

                    word.IsApproved = true;

                    context.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }
        public bool DeleteWords(List<int> wordIds)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                try
                {
                    // Tìm tất cả các từ trong bảng Dictionaries với WordID tương ứng
                    var wordsToDelete = context.Dictionaries
                                               .Include(d => d.Meanings) // Bao gồm các nghĩa liên quan
                                               .Include(d => d.Categories) // Bao gồm các danh mục liên quan
                                               .Where(d => wordIds.Contains(d.WordId))
                                               .ToList();

                    // Nếu không có từ nào tồn tại, trả về false
                    if (!wordsToDelete.Any())
                    {
                        return false;
                    }

                    foreach (var word in wordsToDelete)
                    {
                        context.Meanings.RemoveRange(word.Meanings);
                        word.Categories.Clear();
                    }

                    context.Dictionaries.RemoveRange(wordsToDelete);

                    context.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }



        public bool UpdateWord(int wordId, string englishWord,string typeOfWord, List<string> categoryNames,
                       string pronunciation, int createdByUserId, string englishMeaning,
                       string vietnameseMeaning, string exampleSentence)
        {
            using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
            {
                try
                {
                    // Tìm từ cần cập nhật trong cơ sở dữ liệu
                    var wordToUpdate = context.Dictionaries
                                              .Include(d => d.Meanings)
                                              .Include(d => d.Categories)
                                              .FirstOrDefault(d => d.WordId == wordId);

                    if (wordToUpdate == null)
                    {
                        MessageBox.Show("Word not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    // Cập nhật các trường khác của từ (trừ IsApproved)
                    wordToUpdate.EnglishWord = englishWord;
                    wordToUpdate.TypeOfWord = typeOfWord;
                    wordToUpdate.Pronunciation = pronunciation;
                    wordToUpdate.CreatedBy = createdByUserId;
                    wordToUpdate.CreatedDate = DateTime.Now; // Cập nhật lại thời gian tạo

                    // Cập nhật nghĩa của từ
                    var meaningToUpdate = wordToUpdate.Meanings.FirstOrDefault();
                    if (meaningToUpdate != null)
                    {
                        meaningToUpdate.EnglishMeaning = englishMeaning;
                        meaningToUpdate.VietnameseMeaning = vietnameseMeaning;
                        meaningToUpdate.ExampleSentence = exampleSentence;
                    }

                    // Cập nhật danh mục
                    wordToUpdate.Categories.Clear(); // Xóa các liên kết danh mục hiện tại

                    foreach (var categoryName in categoryNames)
                    {
                        var category = context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);

                        if (category != null)
                        {
                            wordToUpdate.Categories.Add(category);
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public Dictionary GetWordById(int id)
        {
            using (var context = new PersonalDictionaryDBContext())
            {
                // Tìm từ theo ID và bao gồm các liên kết với nghĩa và danh mục
                var word = context.Dictionaries
                    .Include(d => d.Meanings)
                    .Include(d => d.Categories)
                    .FirstOrDefault(d => d.WordId == id);

                if (word == null)
                {
                    return null; // Trả về null nếu không tìm thấy từ
                }

                // Trả về đối tượng từ điển với các trường được yêu cầu
                return new Dictionary
                {
                    WordId = word.WordId,
                    EnglishWord = word.EnglishWord,
                    Pronunciation = word.Pronunciation,
                    TypeOfWord = word.TypeOfWord,
                    IsApproved = word.IsApproved,
                    CreatedBy = word.CreatedBy,
                    CreatedDate = word.CreatedDate,
                    Meanings = word.Meanings.Select(m => new Meaning
                    {
                        MeaningId = m.MeaningId,
                        EnglishMeaning = m.EnglishMeaning,
                        VietnameseMeaning = m.VietnameseMeaning,
                        ExampleSentence = m.ExampleSentence
                    }).ToList(),
                    Categories = word.Categories.Select(c => new Category
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList()
                };
            }

        }

    }
}
