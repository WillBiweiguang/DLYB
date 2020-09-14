using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Web.Domain.ModelsView;

namespace Infrastructure.Web.Domain.Entity
{
    /// <summary>
    /// model mapping register
    /// </summary>
    public class ModelMap
    {
        /// <summary>
        /// model mapping register
        /// </summary>
        public static void MapperRegister()
        {
            //Identity
            // Mapper.CreateMap<ArticleInfoView, ArticleInfo>();


            Mapper.CreateMap<SysRoleMenuView, SysRoleMenuModel>();
            Mapper.CreateMap<SysConfigView, SysConfigModel>();
            Mapper.CreateMap<SysMenuView, SysMenu>();

            Mapper.CreateMap<SysUserView, SysUser>();
            Mapper.CreateMap<SysRoleView, SysRole>();
            //  Mapper.CreateMap<ArticleThumbsUpView, ArticleThumbsUp>();
            // Mapper.CreateMap<SysUserView, SysUser>();
            Mapper.CreateMap<LogsView, LogsModel>();
            //   Mapper.CreateMap<FeedbackView, Feedback>();
            Mapper.CreateMap<CategoryView, Category>();
            //   Mapper.CreateMap<WechatUserView, WechatUser>();
            //   Mapper.CreateMap<ToolsView, Tools>();
            Mapper.CreateMap<DemoView, Demo>();
            Mapper.CreateMap<AddressView, Address>();
            Mapper.CreateMap<ProjectView, Project>();
            Mapper.CreateMap<BeamInfoView, BeamInfo>();
            Mapper.CreateMap<WeldingView, Welding>();
            Mapper.CreateMap<WeldGeometryView, WeldGeometry>();
            Mapper.CreateMap<WeldLocationView, WeldLocation>();
            Mapper.CreateMap<WeldCategoryLabelingView, WeldCategoryLabeling>();
            Mapper.CreateMap<ThicknessView, Thickness>();
            Mapper.CreateMap<HanJieLocationView, HanJieLocation>();
            Mapper.CreateMap<GrooveTypeView, GrooveTypes>();
            Mapper.CreateMap<WeldCategoryStatisticsView, WeldCategoryStatistics>();
            Mapper.CreateMap<WeldCategoryStatisticsViewModel, WeldCategoryStatisticsV>();
            Mapper.CreateMap<HistoricalCostView, HistoricalCost>();
        }

    }
}
