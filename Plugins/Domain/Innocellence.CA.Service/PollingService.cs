using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility;
using Innocellence.CA.Contracts.CommonEntity;
using Innocellence.CA.Contracts.Contracts;
using Innocellence.CA.Contracts.Entity;
using Innocellence.CA.Contracts.ViewModel;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Service.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
namespace Innocellence.CA.Service
{
    public class PollingService : BaseService<PollingEntity>, IPollingService
    {
        private IPollingQuestionService _pollingQuestionService;
        private IPollingOptionService _pollingOptionService;
        private IPollingResultService _pollingResultService;

        public PollingService(IPollingQuestionService pollingQuestionService,
            IPollingOptionService pollingOptionService,
            IPollingResultService pollingResultService)
            : base("CAAdmin")
        {
            _pollingQuestionService = pollingQuestionService;
            _pollingOptionService = pollingOptionService;
            _pollingResultService = pollingResultService;
        }

        public List<PollingView> QueryList(Expression<Func<PollingEntity, bool>> predicate)
        {
            var lst = Repository.Entities.Where(predicate).Where(a => a.IsDeleted != true).OrderByDescending(x => x.Id).ToList()
                .Select(n => (PollingView)(new PollingView().ConvertAPIModel(n)))
                .ToList();
            return lst;
        }

        public override int InsertView<T>(T objModalSrc)
        {
            int iRet;
            var objModal = (PollingView)(IViewModel)objModalSrc;

            Guid myCode = Guid.NewGuid();
            objModal.GuiId = myCode;
            iRet = base.InsertView(objModal);
            return iRet;
        }

        public int InsertOrUpdateView<T>(T objModalSrc)
        {
            var polling = objModalSrc as PollingView;
            if (polling == null)
            {
                return 0;
            }

            if (polling.Id == 0)
            {
                return InsertView(polling);
            }
            else
            {
                return UpdateView(polling);
            }
        }

        public override int UpdateView<T>(T objModalSrc)
        {
            int iRet;
            var pollingView = (PollingView)(IViewModel)objModalSrc;
            // 在后台根据传过来的值区分哪些option or question是插入哪些是更新

            // 先从后台拉一下当前的数据，组一个oldPollingView
            PollingView oldPollingView = new PollingView();
            oldPollingView.ConvertAPIModel(Repository.Entities.AsNoTracking().FirstOrDefault(a => a.Id == pollingView.Id && a.IsDeleted != true));
            oldPollingView.PollingQuestions = _pollingQuestionService.GetPollingQuestion(pollingView.Id);
            foreach (var oldQuestion in oldPollingView.PollingQuestions)
            {
                oldQuestion.PollingOptionEntities = _pollingOptionService.GetPollingOptions(oldQuestion.Id);
            }

            // 因为直接更新会级联更新，所以这里把孩子都删了更新，然后再加上孩子们
            var newQuestions = pollingView.PollingQuestions;
            pollingView.PollingQuestions = null;
            base.UpdateView(pollingView, new List<string> { "Name", "Type", "StartDateTime", "EndDateTime", "StandardScore", "ReplyMessage" });
            pollingView.PollingQuestions = newQuestions;

            // 检查OldPollingView相对于NewPollingView删除了哪些
            foreach (var oldQuestion in oldPollingView.PollingQuestions)
            {
                // 如果id不在新polling里，就是删掉了
                var sameQuestionInNew = pollingView.PollingQuestions.FirstOrDefault(a => a.Id == oldQuestion.Id && a.IsDeleted != true);
                if (sameQuestionInNew == null)
                {
                    _pollingQuestionService.Repository.Delete(oldQuestion.Id);
                    // 如果删除了question的话，会自动删除option
                    // 我们再次手动删除answer表里相关的option
                    var answers = _pollingResultService.DeleteByQuestionId(oldQuestion.Id);

                    continue;
                }
                // 如果没删，检查option。
                // 如果option被删了，同时去删除用户填写过答案的answer
                else
                {
                    foreach (var oldOption in oldQuestion.PollingOptionEntities)
                    {
                        if (sameQuestionInNew.PollingOptionEntities == null || sameQuestionInNew.PollingOptionEntities.FirstOrDefault(a => a.Id == oldOption.Id && a.IsDeleted != true) == null)
                        {
                            _pollingOptionService.Repository.Delete(oldOption.Id);
                            _pollingResultService.DeleteByOptionId(oldOption.Id);
                            continue;
                        }
                    }
                }
            }

            // 除了删除的，就是添加和更新的
            // 检查NewPollingView相对于OldPollingView添加了哪些
            foreach (var newQuestion in pollingView.PollingQuestions)
            {
                // 如果id是0，肯定新加的，执行级联添加
                if (newQuestion.Id == 0)
                {
                    newQuestion.PollingId = pollingView.Id;
                    _pollingQuestionService.InsertView(newQuestion);
                    continue;
                }
                // 如果不是0，更新自己，并且检查option
                else
                {
                    var options = newQuestion.PollingOptionEntities;
                    newQuestion.PollingOptionEntities = null;
                    _pollingQuestionService.UpdateView(newQuestion);
                    newQuestion.PollingOptionEntities = options;

                    if (newQuestion.PollingOptionEntities != null && newQuestion.PollingOptionEntities.Any())
                    {
                        foreach (var newOption in newQuestion.PollingOptionEntities)
                        {
                            if (newOption.Id == 0)
                            {
                                newOption.QuestionId = newQuestion.Id;
                                _pollingOptionService.InsertView(newOption);
                            }
                            else
                            {
                                _pollingOptionService.UpdateView(newOption, new List<string> { "OptionName", "Picture", "Type" });
                            }
                        }
                    }
                }
            }

            return 1;
        }

        public PollingView GetPollingView(Func<PollingEntity, bool> query, string lillyid = null)
        {
            var polling = Repository.Entities.AsNoTracking().Where(a => a.IsDeleted != true).FirstOrDefault(query);

            if (polling != null)
            {
                var pollingview = (PollingView)new PollingView().ConvertAPIModel(polling);
                var pqv = new List<PollingQuestionView>();

                if (polling.PollingQuestions != null)
                {
                    var pollingQuestion = polling.PollingQuestions.Where(a => a.IsDeleted != true).OrderBy(a => a.OrderIndex).ThenBy(a => a.Id).ToList();

                    foreach (PollingQuestionEntity poll in pollingQuestion)
                    {
                        var pollingQuestionView = (PollingQuestionView)new PollingQuestionView().ConvertAPIModel(poll);
                        var pov =poll.PollingOptionEntities.Where(a => a.IsDeleted != true)
                                .Select(polloption => (PollingOptionView)new PollingOptionView().ConvertAPIModel(polloption))
                                .OrderBy(a => a.OrderIndex).ThenBy(a => a.Id)
                                .ToList();
                        GetOptionPercent(pov, polling.Id, lillyid); //给每个选项都加上百分数和票数
                        pollingQuestionView.PollingOptionEntities = pov;
                        pollingQuestionView.PollingQuestionResult = GetResultEntity(polling.Id, poll.Id, lillyid);
                        //答对人数,只适用于有奖问答
                        if (pollingview.Type == 1)
                        {
                            var lst = PollingResultPerson(polling.Id);
                            lst.Where(a => a.QuestionId == poll.Id).ToList().ForEach(y =>
                            {
                                pollingQuestionView.rightPersons = y.RightPersons;
                                pollingQuestionView.answerPersons = y.answerPersons;
                            });
                        }
                        pqv.Add(pollingQuestionView);
                    }
                }

                pollingview.PollingQuestions = pqv;
                pollingview.PollingTotal =
                    _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingview.Id && x.IsDeleted != true)
                        .Select(x => x.UserId.ToUpper())
                        .Distinct()
                        .Count();
                //pollingview.StandardScore = GetScore(polling.Id);
                return pollingview;
            }

            return new PollingView();
        }

        public PollingView GetPollingDetailView(Func<PollingEntity, bool> query, string lillyid = null)
        {
            PollingView pollingview = GetPollingView(query, lillyid);
            if (pollingview.PollingQuestions != null && pollingview.PollingQuestions.Count > 0)
            {
                pollingview.PollingQuestions.ToList().ForEach(x =>
                {
                    if (x.Type == 1)
                    {
                        x.optionName = "(单选)";

                    }
                    else if (x.Type == 0 || x.Type == x.PollingOptionEntities.Count)
                    {
                        x.optionName = "(多选)";
                    }
                    else if (x.Type < x.PollingOptionEntities.Count)
                    {
                        x.optionName = "(最多选" + x.Type + "项)";
                    }
                    else if (x.Type == 999)
                    {
                        x.optionName = "文本";
                    }
                });
            }
            if (pollingview.Type == 1)
            {
                pollingview.Title = "有奖问答";
            }
            else if (pollingview.Type == 2)
            {
                pollingview.Title = "投票";
            }
            if (pollingview.StartDateTime > DateTime.Now)
            {
                pollingview.StatusName = "未开始";
            }
            else if (pollingview.EndDateTime > DateTime.Now && pollingview.StartDateTime < DateTime.Now)
            {
                pollingview.StatusName = "提交";
            }
            else if (pollingview.EndDateTime < DateTime.Now)
            {
                pollingview.StatusName = "已结束";
            }


            return pollingview;
        }

        public string GetResultEntity(int pollingid, int questionid, string lillyid)
        {
            var answer =
                   _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingid && x.Answer == 0 && x.QuestionId == questionid && x.UserId == lillyid && x.IsDeleted != true)

                       .Select(x => x.AnswerText)
                       .FirstOrDefault();

            return answer;
        }

        public List<PollingOptionView> GetOptionPercent(List<PollingOptionView> pov, int pollingid, string lillyid)
        {
            var allAnswers =
                _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingid && x.IsDeleted != true)
                    .Select(x => new { UserId = x.UserId, QuestionId = x.QuestionId, Answer = x.Answer, AnswerText = x.AnswerText })
                    .ToList();

            allAnswers.ForEach(x => pov.ForEach(z =>
            {
                if (x.UserId == lillyid && z.Id == x.Answer)
                {
                    z.SelectName = "(已选)";
                }
            }));

            allAnswers.GroupBy(x => x.QuestionId).ToList().ForEach(x =>
            {
                int total = x.Count();
                x.GroupBy(y => y.Answer).ToList().ForEach(y =>
                {
                    int answer = y.First().Answer;
                    int optionCount = y.Count();
                    int perent = (optionCount * 100 / total);
                    pov.ForEach(z =>
                    {
                        if (z.Id != answer) return;
                        z.VoteNum = optionCount;
                        z.Percent = perent;
                    });
                });
            });

            return pov;
        }

        public PollingScreenView GetPollingScreenData(int pollingid, int questionid)
        {
            var pollingScreenView = new PollingScreenView();

            var allAnswers = _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingid && x.QuestionId == questionid && x.IsDeleted != true)
                    .Select(x => new { UserId = x.UserId, QuestionId = x.QuestionId, Answer = x.Answer, AnswerText = x.AnswerText })
                    .ToList();

            var total = _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingid && x.IsDeleted != true)
                        .Select(x => x.UserId.ToUpper()).Distinct().Count();

            pollingScreenView.PollingTotal = total;//最新的投票人数
            var sereisLst = new List<SereisData>();

            var allOptions = _pollingOptionService.Repository.Entities.Where(x => x.QuestionId == questionid && x.IsDeleted != true)
                .Select(x => new { Id = x.Id, OptionName = x.OptionName }).ToList();


            var options = allOptions.AsParallel().Where(x => allAnswers.AsParallel().All(y => x.Id != y.Answer)).Select(x => new SereisData { Sereisname = x.OptionName, Sereisvalue = 0 }).ToList();

            sereisLst.AddRange(options);

            allAnswers.GroupBy(y => y.Answer).ToList().ForEach(y =>
            {
                int answer = y.First().Answer;
                int optionCount = y.Count();
                int perent = (optionCount * 100 / allAnswers.Count());
                var obj = new SereisData()
                {
                    Sereisname = allOptions.Find(x => x.Id == answer).OptionName,
                    Sereisvalue = perent
                };
                sereisLst.Add(obj);
            });

            pollingScreenView.SereisData = sereisLst;

            return pollingScreenView;
        }

        public string ConvertAbcToYn(string abc, int len = 10)
        {
            return ConvertAbcToYnStatic(abc, len);
        }

        public static string ConvertAbcToYnStatic(string abc, int len = 10)
        {
            var ync = new char[10] { 'N', 'N', 'N', 'N', 'N', 'N', 'N', 'N', 'N', 'N' };
            if (string.IsNullOrEmpty(abc))
            {
                return new string(ync);
            }
            if (abc.Length > 10)
            {
                abc = abc.Substring(0, 10);
            }
            foreach (var c in abc.ToUpper())
            {
                int i = c - 'A';
                if (i >= 10)
                {
                    continue;
                }
                ync[i] = 'Y';
            }

            return new string(ync);
        }

        public static string ConvertYNToABCStatic(string YN, int len = 10)
        {
            var ync = new char[10] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
            var result = "";
            if (string.IsNullOrEmpty(YN))
            {
                return result;
            }
            if (YN.Length > 10)
            {
                YN = YN.Substring(0, 10);
            }

            for (int i = 0; i < YN.Length - 1; i++)
            {
                if (YN[i] == 'Y')
                {
                    result += ync[i].ToString();
                }
            }

            return result;
        }

        //答题列表
        public List<PollingResultCustomView> PollingResult(int pollingId)
        {
            // 1. 取出polling，创建每个question和answer的列表
            var polling = Repository.Entities.FirstOrDefault(a => a.Id == pollingId && (a.IsDeleted == null || a.IsDeleted == false));
            if (polling == null)
            {
                throw new Exception("Polling为空！");
            }

            var pollingCustomView = new PollingCustomView { PollingId = pollingId };


            foreach (var question in polling.PollingQuestions)
            {
                var pollingQuestions = question.PollingOptionEntities.Where(a => (a.IsDeleted == null || a.IsDeleted==false));
                for (int i = 0; i < pollingQuestions.Count(); i++)
                {
                    var option = question.PollingOptionEntities.ElementAt(i);
                    option.OptionIndex = i.ToString();//toABCD(i);
                }
            }

            // 开始处理答案
            var pollingAnswers = _pollingResultService.Repository.Entities.Where(a => a.PollingId == pollingId && (a.IsDeleted == null || a.IsDeleted == false));

            // 同一个人对同一个question的答案（多选），需要汇总到一个里。

            foreach (var pollingAnswer in pollingAnswers)
            {
                var question = polling.PollingQuestions.FirstOrDefault(a => a.Id == pollingAnswer.QuestionId);
                if (question != null && question.Type == 99)
                {
                    continue;
                }

                // 如果是多选题的话，需要看是否已经有过，有过的话就合并答案
                //var newAnswerResult = pollingCustomView.Results.FirstOrDefault(a => a.QuestionId == question.Id && a.UserId == pollingAnswer.UserId);
                var newAnswerResult = pollingCustomView.Results.FirstOrDefault(a => a.QuestionId == question.Id && a.UserId.Equals(pollingAnswer.UserId,StringComparison.InvariantCultureIgnoreCase));
                var empDetails = WeChatCommonService.lstUserWithDeptTag;
                if (newAnswerResult == null)
                {
                    var emp = empDetails.SingleOrDefault(a => a.userid.Equals(pollingAnswer.UserId,StringComparison.InvariantCultureIgnoreCase));

                    if (question != null)
                        newAnswerResult = new PollingResultCustomView()
                        {
                            QuestionId = question.Id,
                            QuestionTitle = question.Title,
                            UserId = pollingAnswer.UserId,

                            UserName = (emp != null ? emp.name : ""),
                            UserDeptLv1 = (emp != null ? emp.deptLvs[2] : ""),
                            UserDeptLv2 = (emp != null ? emp.deptLvs[3] : ""),
                            UserDeptLv3 = (emp != null ? emp.deptLvs[4] : ""),
                            AnswerTime = pollingAnswer.CreatedDate.GetValueOrDefault()
                        };
                    pollingCustomView.Results.Add(newAnswerResult);
                }

                if (question != null)
                {
                    var option = question.PollingOptionEntities.FirstOrDefault(a => a.Id == pollingAnswer.Answer && (a.IsDeleted == null || a.IsDeleted == false));
                    if (option == null)
                    {
                        continue;
                    }
                    int optionIndex = int.Parse(option.OptionIndex);
                    var answer = newAnswerResult.Answers[optionIndex];

                    newAnswerResult.Answers[optionIndex].AnswerId = pollingAnswer.Answer;
                    newAnswerResult.Answers[optionIndex].AnswerText = pollingAnswer.AnswerText;
                }
            }

            foreach (var questionresult in pollingCustomView.Results)
            {
                var answered = questionresult.Answers.Aggregate("", (current, answer) => current + (answer.AnswerId == 0 ? "N" : "Y"));

                questionresult.CustomAnswer = ConvertYNToABCStatic(answered, 10);
                var questionstr = polling.PollingQuestions.FirstOrDefault(a => a.Id == questionresult.QuestionId);
                if (questionstr != null) questionresult.RightAnswers = questionstr.RightAnswers;
                questionresult.CustomStatus = questionresult.CustomAnswer == questionresult.RightAnswers ? "正确" : "错误";
            }
            return pollingCustomView.Results;
        }

        //得分列表
        public List<PollingResultScoreView> PollingResultScore(int pollingId)
        {
            var polling = Repository.Entities.FirstOrDefault(a => a.Id == pollingId);
            var pollingResultScores = new List<PollingResultScoreView>();

            var pollingAnswers = _pollingResultService.Repository.Entities.Where(a => a.PollingId == pollingId).Where(x => x.UserId != null).ToList();
            pollingAnswers.GroupBy(x => x.UserId).ToList().ForEach(y =>
            {
                var pollingResultScore = new PollingResultScoreView();
                int score = 0;
                foreach (var questionresult in PollingResult(pollingId))
                {
                    var answered = questionresult.Answers.Aggregate("", (current, answer) => current + (answer.AnswerId == 0 ? "N" : "Y"));

                    if (polling != null)
                    {
                        var questionstr = polling.PollingQuestions.FirstOrDefault(a => a.Id == questionresult.QuestionId);

                        if (questionstr != null)
                        {
                            var answer1 = ConvertAbcToYn(questionstr.RightAnswers, 10);

                            if (answer1 == answered && questionresult.UserId.ToUpper() == y.Key.ToUpper())
                            {
                                score += questionstr.Score.GetValueOrDefault();
                            }
                            else
                            {
                                score += 0;
                            }
                        }
                    }
                }
                List<EmployeeInfoWithDept> empDetails = WeChatCommonService.lstUserWithDeptTag;
                var emp = empDetails.SingleOrDefault(a => a.userid.ToUpper().Equals(y.Key.ToUpper()));
                pollingResultScore.UserId = y.Key;
                pollingResultScore.CustomScore = score;
                if (emp != null)
                {
                    pollingResultScore.UserName = emp.name;
                    pollingResultScore.UserDeptLv1 = emp.deptLvs[2];
                    pollingResultScore.UserDeptLv2 = emp.deptLvs[3];
                    pollingResultScore.UserDeptLv3 = emp.deptLvs[4];
                }
                pollingResultScores.Add(pollingResultScore);
            });

            return pollingResultScores;
        }

        //答题正确人数
        public List<PollingResultCustomView> PollingResultPerson(int pollingId)
        {
            var polling = Repository.Entities.FirstOrDefault(a => a.Id == pollingId);
            var pollingResultPersons = new List<PollingResultCustomView>();

            var pollingAnswers = _pollingResultService.Repository.Entities.Where(a => a.PollingId == pollingId).Where(x => x.QuestionId != null).ToList();
            pollingAnswers.GroupBy(x => x.QuestionId).ToList().ForEach(y =>
            {
                var pollingResultPerson = new PollingResultCustomView();
                int person = 0;
                int answerPersons = 0;
                foreach (var questionresult in PollingResult(pollingId))
                {
                    if (questionresult.CustomStatus.Equals("正确") && questionresult.QuestionId == y.Key)
                    {
                        person += 1;
                        answerPersons += 1;
                    }
                    if (questionresult.CustomStatus.Equals("错误") && questionresult.QuestionId == y.Key)
                    {
                        answerPersons += 1;
                    }
                }
                pollingResultPerson.QuestionId = y.Key;
                pollingResultPerson.RightPersons = person;
                pollingResultPerson.answerPersons = answerPersons;
                pollingResultPersons.Add(pollingResultPerson);
            });

            return pollingResultPersons;
        }

        //每个人得分情况
        public int GetScore(int pollingId, string lillyid)
        {
            string answered = "";
            int score = 0;
            var answer1 = "";
            var polling = Repository.Entities.FirstOrDefault(a => a.Id == pollingId);
            foreach (var questionresult in PollingResult(pollingId))
            {

                answered = questionresult.Answers.Aggregate("", (current, answer) => current + (answer.AnswerId == 0 ? "N" : "Y"));

                if (polling != null)
                {
                    var questionstr = polling.PollingQuestions.FirstOrDefault(a => a.Id == questionresult.QuestionId);

                    if (questionstr != null)
                    {
                        answer1 = ConvertAbcToYn(questionstr.RightAnswers, 10);
                        if (answer1 == answered && questionresult.UserId.ToUpper() == lillyid.ToUpper())
                        {
                            score += questionstr.Score.GetValueOrDefault();
                        }
                        else
                        {
                            score += 0;
                        }
                    }
                }
            }
            return score;

        }

    }

    public class PollingComapre : IEqualityComparer<PollingResultEntity>
    {

        public bool Equals(PollingResultEntity x, PollingResultEntity y)
        {
            return x.UserId == y.UserId;
        }

        public int GetHashCode(PollingResultEntity obj)
        {
            return obj.UserId.GetHashCode();
        }
    }
}
