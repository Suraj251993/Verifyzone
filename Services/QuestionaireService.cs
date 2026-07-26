using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrgCheck.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;

namespace OrgCheck.Services
{
    public class QuestionaireService : IQuestionaireService
    {
        private readonly IServiceProvider _serviceProvider;
        public QuestionaireService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public List<SelectListItem> GetQuestions()
        {
            var list = _serviceProvider.GetRequiredService<IQuestionaireDA>().GetQuestions();
            return list.Select(_ => new SelectListItem()
            {
                Value = _.Id.ToString(),
                Text = _.Question
            }).ToList();
        }
        public QuestionViewModel GetQuestion(int id)
        {
            var record = _serviceProvider.GetRequiredService<IQuestionaireDA>().GetQuestion(id);
            return new QuestionViewModel()
            {
                Id = record.Id,
                Question = record.Question
            };
        }
        public string AddQuestion(QuestionViewModel item)
        {
            var record = new Questionaire()
            {
                Question = item.Question
            };
            if (_serviceProvider.GetRequiredService<IQuestionaireDA>().IsDuplicateQuestion(0, item.Question))
                return "exists";
            _serviceProvider.GetRequiredService<IQuestionaireDA>().AddQuestion(record);
            return "true";
        }
        public string UpdateQuestion(QuestionViewModel item)
        {
            var record = new Questionaire()
            {
                Id = item.Id,
                Question = item.Question
            };
            if (_serviceProvider.GetRequiredService<IQuestionaireDA>().IsDuplicateQuestion(item.Id, item.Question))
                return "exists";
            _serviceProvider.GetRequiredService<IQuestionaireDA>().UpdateQuestion(record);
            return "true";
        }
        public List<SelectListItem> GetQuestionsByCompany(int companyId)
        {
            var list = _serviceProvider.GetRequiredService<IQuestionaireDA>().GetQuestionsByCompany(companyId);
            return list.Select(_ => new SelectListItem()
            {
                Value = _.Id.ToString(),
                Text = _.Question
            }).ToList();
        }
        public List<QuestionaireMappingViewModel> GetQuestionaireMappingByCompany(int companyId)
        {
            var result = new List<QuestionaireMappingViewModel>();
            var list = GetQuestions();
            var companylist = GetQuestionsByCompany(companyId);
            foreach(var item in list)
            {
                var _exists = companylist.Where(_ => _.Value == item.Value).FirstOrDefault();
                result.Add(new QuestionaireMappingViewModel()
                {
                    Id = item.Value,
                    Question = item.Text,
                    QuestionId = item.Value,
                    CompanyId = Convert.ToString(companyId),
                    IsSelected = (_exists != null ? true : false)
                });
            }
            return result;
        }
        public bool AddQuestionareMapping(List<QuestionaireMappingViewModel> newlist)
        {
            var list = new List<Companyquestion>();
            foreach(var item in newlist)
            {
                if (item.IsSelected)
                {
                    list.Add(new Companyquestion()
                    {
                        Companyid = Convert.ToInt32(item.CompanyId),
                        Questionid = Convert.ToInt32(item.QuestionId),
                    });
                }
            }
            _serviceProvider.GetRequiredService<IQuestionaireDA>().AddQuestionareMapping(list);
            return true;
        }
    }
}
