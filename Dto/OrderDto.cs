using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Dto
{
    public class OrderDto
    {
        public List<OrderFromCartDto>? CartItems { get; set; }
    }
}