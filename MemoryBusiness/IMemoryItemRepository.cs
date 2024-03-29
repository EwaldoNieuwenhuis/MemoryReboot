using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessMemory
{
    // Een interface in de context van objectgeoriënteerd programmeren is een contract dat
    // een set van methoden, eigenschappen, gebeurtenissen of indexers definieert die een klasse
    // moet implementeren.Een interface specificeert wat een klasse moet doen, maar niet hoe het moet worden gedaan.
    // Het biedt een abstracte blauwdruk voor functionaliteit die kan worden gedeeld tussen verschillende klassen.
    public interface IMemoryItemRepository<T>
    {
        void Create(T item, out int rowsAffected, out T newRecord);
        void Update(int id, T item, out int rowsAffected, out T updatedRecord);
        void Delete(int id, out int rowsAffected);
        T Read(int id);
        
    }
}

