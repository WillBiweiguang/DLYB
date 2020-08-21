using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace DLYB.CA.Contracts.Entity
{
    [Table("LocalSAD")]
    public class LocalSADEntity : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public long GlobalID { get; set; }
        public string LillyID { get; set; }
        public string ChineseName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }

        [NotMapped]
        public string Manager { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactNumber { get; set; }

        public string Title { get; set; }
        public string ManagerID { get; set; }
        public string ManagerName { get; set; }
        public long STTS_IND { get; set; }
        public DateTime CRT_DT { get; set; }
        public string CRT_USR { get; set; }
        public DateTime UP_DT { get; set; }
        public string UP_USR { get; set; }

        [NotMapped]
        public string ManagerTel { get; set; }
        //public string ManagerLillyId { get; set; }

        [NotMapped]
        public int DeptId
        {
            get
            {

                //if (!string.IsNullOrEmpty(SubDepartment))
                //{
                //    return int.Parse(SubDepartment);
                //}

                //if (!string.IsNullOrEmpty(Department))
                //{
                //    return int.Parse(Department);
                //}

                //return !string.IsNullOrEmpty(Company) ? int.Parse(Company) : 0;
                return 0;
            }
        }

        public string Company { get; set; }
        public string Department { get; set; }
        public string SubDepartment { get; set; }
        //public List<string> Tags { get; set; }
        public string SFTitle { get; set; }
        public string IsSupervisor { get; set; }
        public string Contractorgroup { get; set; }
        public string BaseLocation { get; set; }
        public DateTime? SyncTime { get; set; }
        public string ChangeType { get; set; }

    }


    public class LocalSADEntityQueryResult
    {
        public List<LocalSADEntity> NewEmployees
        {
            get;
            set;
        }

        public List<LocalSADEntity> UpdatedEmployees
        {
            get;
            set;
        }

    }
}
