using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Sync
{
    public interface IProxy
    {
        EndpointAddress setDadosAutenticacao(EndpointAddress Adress);
    }
}
