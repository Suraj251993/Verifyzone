using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;

namespace OrgCheck.DataAccess
{
    public class QuestionaireDA : IQuestionaireDA
    {
        public PostgresContext orgCheckContext;
        public QuestionaireDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }
        public List<Questionaire> GetQuestions()
        {
            return orgCheckContext.Questionaires.AsNoTracking().Where(_ => _.Status == 1).OrderBy(_ => _.Id).ToList();
        }
        public Questionaire GetQuestion(int Id)
        {
            return orgCheckContext.Questionaires.AsNoTracking().Where(_ => _.Id == Id).FirstOrDefault();
        }
        public void AddQuestion(Questionaire questionaire)
        {
            orgCheckContext.Questionaires.Add(questionaire);
            orgCheckContext.SaveChanges();
        }
        public void UpdateQuestion(Questionaire questionaire)
        {
            var existingEntity = orgCheckContext.Questionaires.FirstOrDefault(_ => _.Id == questionaire.Id);
            existingEntity.Question = questionaire.Question;
            orgCheckContext.SaveChanges();
        }
        public List<Questionaire> GetQuestionsByCompany(int companyId)
        {
            return orgCheckContext.Companyquestions.Include(x => x.Question).AsNoTracking()
                .Where(_ => _.Companyid == companyId).Select(_ => _.Question).OrderBy(_ => _.Id).ToList();
        }
        public void AddQuestionareMapping(List<Companyquestion> newlist)
        {
            int _companyId = newlist[0].Companyid;
            var existinglist = orgCheckContext.Companyquestions.AsNoTracking().Where(_ => _.Companyid == _companyId)
                .OrderBy(_ => _.Questionid).ToList();

            var deleteList = existinglist.Where(_ => !newlist.Any(d => _.Questionid == d.Questionid)).ToList();
            var addList = newlist.Where(_ => !existinglist.Any(d => _.Questionid == d.Questionid)).ToList();

            // Deleting the additional in the table
            foreach (var deleteRecord in deleteList)
                orgCheckContext.Companyquestions.Remove(deleteRecord);

            // Adding new entries in the table
            foreach (var add in addList)
            {
                orgCheckContext.Companyquestions.Add(new Companyquestion()
                {
                    Companyid = _companyId,
                    Questionid = add.Questionid
                });
            }

            orgCheckContext.SaveChanges();
        }
        public bool IsDuplicateQuestion(int id, string name)
        {
            bool _result = false;
            var question = new Questionaire();
            if (id > 0)
                question = orgCheckContext.Questionaires.AsNoTracking().Where(_ => _.Question.ToUpper()
                    .Equals(name.ToUpper()) && _.Id != id && _.Status == 1).FirstOrDefault();
            else
                question = orgCheckContext.Questionaires.AsNoTracking().Where(_ => _.Question.ToUpper()
                    .Equals(name.ToUpper()) && _.Status == 1).FirstOrDefault();
            if ( question != null && question.Id > 0)
                _result = true;

            return _result;
        }
    }
}
