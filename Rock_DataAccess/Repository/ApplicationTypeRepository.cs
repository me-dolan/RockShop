using Rock_DataAccess.Repository.IRepository;
using Rock_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationTypeRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }

        public void Update(ApplicationType obj)
        {
            var objFromDb = _context.Categories.FirstOrDefault(e => e.Id == obj.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }
        }
    }
}
