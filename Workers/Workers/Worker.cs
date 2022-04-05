
namespace Workers
{
	class Worker
	{
		/* The class that stores the data of each worker. Used in DataGrid. */

		public Worker(string _id, string _sur, string _n, string _p, string _d, string _s, string _h)
		{
			this.id = _id;
			this.surname = _sur;
			this.name = _n;
			this.patronymic = _p;
			this.date_of_birthday = _d;
			this.sex = _s;
			this.having_children = _h;
		}

		public string id { get; set; }
		public string surname { get; set; }
		public string name { get; set; }
		public string patronymic { get; set; }
		public string date_of_birthday { get; set; }
		public string sex { get; set; }
		public string having_children { get; set; }
	}
}
