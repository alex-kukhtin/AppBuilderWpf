using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGenerator.Interfaces;

public interface ISqlGenerator
{
	void Generate(TableDescriptor descr, AppElem appElem);
	void Finish();
}
