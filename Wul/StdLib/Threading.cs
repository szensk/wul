using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    class Threading
    {
        private static Task NetObjectToTask(NetObject no)
        {
            var taskObject = no.ToObject();
            if (taskObject is Task task)
            {
                return task;
            }
            return null;
        }

        [NetFunction("task")]
        private static IValue NewTask(List<IValue> list, Scope scope)
        {
            var first = list[0];
            if (first is Function func)
            {
                var args = list.Skip(1).ToList();
                return new NetObject(new Task<IValue>((Func<IValue>) func.ToObject(args)));
            }
            return Value.Nil;
        }

        private static IValue AwaitInternal(NetObject no)
        {
            var taskObject = no.ToObject();
            if (taskObject is Task<IValue> taskResult)
            {
                if (taskResult.Status == TaskStatus.Created) taskResult.Start();
                taskResult.Wait();
                return taskResult.Result;
            }
            else if (taskObject is Task task)
            {
                if (task.Status == TaskStatus.Created) task.Start();
                task.Wait();
                return Value.Nil;
            }
            throw new Exception("Only functions or taks can be used with await");
        }

        [NetFunction("await")]
        internal static IValue Await(List<IValue> list, Scope scope)
        {
            var first = list[0];
            if (first is NetObject no)
            {
                return AwaitInternal(no);
            }
            else if (first is Function)
            {
                var task = NewTask(list, scope);
                if (task == null) throw new Exception("Unable to create task");
                return AwaitInternal((NetObject) task);
            }
            throw new Exception("Only functions or taks can be used with await");
        }

        [NetFunction("waitall")]
        internal static IValue WaitAll(List<IValue> list, Scope scope)
        {
            var tasks = list.OfType<NetObject>()
                .Select(NetObjectToTask)
                .Where(t => t != null)
                .ToArray();

            if (tasks.Length != list.Count)
            {
                throw new Exception("Wait all only accepts tasks");
            }

            Task.WaitAll(tasks);
            return Value.Nil;
        }

        //TODO WhenAll / WhenAny

        [NetFunction("sleep")]
        internal static IValue Sleep(List<IValue> list, Scope scope)
        {
            var sleeptime = (Number) list[0];
            Thread.Sleep(sleeptime);
            return Value.Nil;
        }
    }
}