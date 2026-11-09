using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DAO
{
    public class MessageAdminDAO : CommonsDAO, IMessageAdminDAO
    {
        public MessageAdminDAO() : base()
        {
        }

        public ResponseInfo<bool> Delete(int Identifier, int idUser = 0)
        {
            throw new NotImplementedException();
        }

        public ResponseInfo<MESSAGE_ADMIN> Create(MESSAGE_ADMIN entity)
        {
            return CreateRegedit(entity);
        }

        public ResponseInfo<MESSAGE_ADMIN> Retrieve(int Identifier)
        {
            throw new NotImplementedException();
        }

        public ResponseInfo<MESSAGE_ADMIN> Update(MESSAGE_ADMIN entityObject)
        {
            throw new NotImplementedException();
        }
    }
}
