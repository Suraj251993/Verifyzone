using OrgCheck.Models;
using System;
using System.Collections.Generic;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface IQuestionaireDA
    {
        List<Questionaire> GetQuestions();
        Questionaire GetQuestion(int Id);
        void AddQuestion(Questionaire questionaire);
        void UpdateQuestion(Questionaire questionaire);
        List<Questionaire> GetQuestionsByCompany(int companyId);
        void AddQuestionareMapping(List<Companyquestion> newlist);
        bool IsDuplicateQuestion(int id, string name);
    }
}
