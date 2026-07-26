using Microsoft.AspNetCore.Mvc.Rendering;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Services.Interfaces
{
    public interface IQuestionaireService
    {
        List<SelectListItem> GetQuestions();
        QuestionViewModel GetQuestion(int id);
        string AddQuestion(QuestionViewModel item);
        string UpdateQuestion(QuestionViewModel item);
        List<SelectListItem> GetQuestionsByCompany(int companyId);
        List<QuestionaireMappingViewModel> GetQuestionaireMappingByCompany(int companyId);
        bool AddQuestionareMapping(List<QuestionaireMappingViewModel> newlist);
    }
}
