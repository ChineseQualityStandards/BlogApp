using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BlogApp.Core.DbContexts;
using BlogApp.Core.Models;
using BlogApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    /// <summary>
    /// 数据库操作服务
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class DatabaseService<T> : IDatabaseService<T> where T : class
    {
        private readonly BlogAppContext _context;

        public DatabaseService(BlogAppContext context)
        {
            _context = context;
        }
        
        public async Task<EFMessage<T>> Add(T t)
        {
            bool flag = false;
            await _context.AddAsync(t);
            if(await  _context.SaveChangesAsync() >= 1)
                flag = true;
            return new EFMessage<T>(flag, t);
        }

        public async Task<EFMessage<T>> Delete(T t)
        {
            bool flag = false;
            string code = string.Empty;
            _context.Remove(t);
            if (await _context.SaveChangesAsync() >= 1)
            {
                flag = true;
                code = "成功删除";
            }
            return new EFMessage<T>(flag,code,t);
        }

        public async Task<EFMessage<ObservableCollection<T>>> Get(int id)
        {
            bool flag = false;
            string code = string.Empty;
            ObservableCollection<T> result = new ObservableCollection<T>();
            // 泛型约束才不会报错 即给本类加上where T : class的约束 把T的类型约束为class
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) 
            {
                code = "未找到对应实体";
            }
            else
            {
                code = "查询成功";
                result.Add(entity);
            }
            return new EFMessage<ObservableCollection<T>>(flag, code, result);
        }

        public async Task<EFMessage<ObservableCollection<T>>> Get(string key, string condition)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return new EFMessage<ObservableCollection<T>>(false, "列名不能为空", null);
                }

                if (condition == null)
                {
                    return new EFMessage<ObservableCollection<T>>(false, "查询条件不能为空", null);
                }

                var query = _context.Set<T>().AsQueryable();

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, key);

                // 确保条件值的类型与属性类型匹配
                var convertedCondition = Convert.ChangeType(condition, property.Type);
                var constant = Expression.Constant(convertedCondition);

                var equals = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

                var results = await query.Where(lambda).ToListAsync();
                var collection = new ObservableCollection<T>(results);

                return new EFMessage<ObservableCollection<T>>(true, "查询成功", collection);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("property"))
            {
                return new EFMessage<ObservableCollection<T>>(false, $"列名 '{key}' 不存在", null);
            }
            catch (InvalidCastException)
            {
                return new EFMessage<ObservableCollection<T>>(false, $"条件值类型与列 '{key}' 的类型不匹配", null);
            }
            catch (Exception ex)
            {
                return new EFMessage<ObservableCollection<T>>(false, $"查询失败: {ex.Message}", null);
            }
        }

        public async Task<EFMessage<T>> Update(T t)
        {
            bool flag = false;
            _context.Update(t);
            if(await _context.SaveChangesAsync() >= 1)
                flag = true;
            return new EFMessage<T>(flag, t);
        }

    }
}
