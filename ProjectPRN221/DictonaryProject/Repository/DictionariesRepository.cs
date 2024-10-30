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
    class DictionariesRepository : IDictionaryRepository
    {
        public Category AddCategory(string categoryName)
        {
            return DictionaryDAO.Instance.AddCategory(categoryName);
        }

        public bool AddNewWord(string englishWord, string type, List<string> categoryNames, string pronunciation, int createdByUserId, string englishMeaning, string vietnameseMeaning, string exampleSentence, bool isUser)
        {
            return DictionaryDAO.Instance.AddNewWord(englishWord,type, categoryNames, pronunciation, createdByUserId, englishMeaning, vietnameseMeaning, exampleSentence, isUser);
        }

      

        public bool ApproveWord(int dictionaryId)
        {
            return DictionaryManagementDAO.Instance.ApproveWord(dictionaryId);
        }

        public bool DeleteWord(int dictionaryId)
        {
            return DictionaryManagementDAO.Instance.DeleteWord(dictionaryId);
        }

        public List<object> dictionaries()
        {
            return DictionaryDAO.Instance.GetAllDictionary();
        }

        public List<Category> GetAllCategories()
        {
           return DictionaryDAO.Instance.GetAllCategories();
        }

        public List<object> GetAllWordsOfAdmin()
        {
            return DictionaryManagementDAO.Instance.GetAllDictionaryManagement();
        }

        public int GetUnapprovedWordCount()
        {
            return DictionaryManagementDAO.Instance.GetUnapprovedWordCount();
        }

        public Dictionary GetWordById(int id)
        {
           return DictionaryManagementDAO.Instance.GetWordById(id);
        }

        public List<object> SearchByWordAndCategory(string keyword, string categoryName)
        {
            return DictionaryDAO.Instance.SearchByWordAndCategory(keyword, categoryName);
        }

        public List<object> SearchByWordAndCategoryForAdmin(string keyword, string categoryName)
        {
           return DictionaryManagementDAO.Instance.SearchByWordAndCategory(keyword, categoryName);
        }

        public bool UpdateWord(int wordId, string englishWord,string typeOfWord, List<string> categoryNames, string pronunciation, int createdByUserId, string englishMeaning, string vietnameseMeaning, string exampleSentence)
        {
            return DictionaryManagementDAO.Instance.UpdateWord(wordId, englishWord,typeOfWord, categoryNames, pronunciation, createdByUserId, englishMeaning, vietnameseMeaning, exampleSentence);
        }

        public bool ValidateWordExistence(string englishWord, string englishMeaning, string vietnameseMeaning)
        {
            return DictionaryDAO.Instance.ValidateWordExistence(englishWord, englishMeaning, vietnameseMeaning);
        }
    }
}
