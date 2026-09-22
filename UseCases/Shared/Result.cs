using System;
using System.Threading.Tasks;

namespace UseCases.Shared
{
    public readonly struct Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(string error) => new Result(false, error);
    }

    public readonly struct Result<T>
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string error) => new Result<T>(false, default, error);
    }

    public static class ResultExtensions
    {
        public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> task, Func<T, Task<Result<U>>> func)
        {
            var res = await task.ConfigureAwait(false);
            if (!res.IsSuccess) return Result<U>.Failure(res.Error ?? "Unknown error");
            return await func(res.Value!).ConfigureAwait(false);
        }

        public static Task<Result<U>> Bind<T, U>(this Result<T> res, Func<T, Task<Result<U>>> func)
        {
            if (!res.IsSuccess) return Task.FromResult(Result<U>.Failure(res.Error ?? "Unknown error"));
            return func(res.Value!);
        }

        public static Result<U> Map<T, U>(this Result<T> res, Func<T, U> fn)
        {
            if (!res.IsSuccess) return Result<U>.Failure(res.Error ?? "Unknown error");
            return Result<U>.Success(fn(res.Value!));
        }

        public static Task<Result<T>> ToTask<T>(this Result<T> res) => Task.FromResult(res);
    }
}
