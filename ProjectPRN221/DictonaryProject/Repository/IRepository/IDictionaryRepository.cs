using DictonaryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject.Repository.IRepository
{
    public interface IDictionaryRepository
    {

        //Dictionary of User
        List<object> dictionaries();

        List<object> SearchByWordAndCategory(string keyword, string categoryName);

        bool AddNewWord(string englishWord, List<string> categoryNames, string pronunciation,
                         int createdByUserId, string englishMeaning, string vietnameseMeaning,
                         string exampleSentence, bool isUser, bool addCategoryIfNotExist);


        bool ValidateWordExistence(string englishWord, string englishMeaning, string vietnameseMeaning);


        //Category
        List<Category> GetAllCategories();

        Category AddCategory(string categoryName);


        //Dictionary of Admin
        bool ApproveWord(int dictionaryId);
        List<Object> GetAllWordsOfAdmin();

        bool DeleteWord(int dictionaryId);

        bool UpdateWord(int wordId, string englishWord, List<string> categoryNames,
                       string pronunciation, int createdByUserId, string englishMeaning,
                       string vietnameseMeaning, string exampleSentence);
        int GetUnapprovedWordCount();


        List<object> SearchByWordAndCategoryForAdmin(string keyword, string categoryName);

        Dictionary GetWordById(int id);
    }
}
