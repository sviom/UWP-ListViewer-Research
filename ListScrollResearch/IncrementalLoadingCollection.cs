using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace ListScrollResearch
{
    /// <summary>
    /// This interface represents a data source whose items can be loaded incrementally.
    /// </summary>
    /// <typeparam name="TSource">Type of collection element.</typeparam>
    public interface IIncrementalSource<TSource>
    {
        /// <summary>
        /// This method is invoked everytime the view need to show more items. Retrieves items based on <paramref name="pageIndex"/> and <paramref name="pageSize"/> arguments.
        /// </summary>
        /// <param name="pageIndex">
        /// The zero-based index of the page that corresponds to the items to retrieve.
        /// </param>
        /// <param name="pageSize">
        /// The number of <typeparamref name="TSource"/> items to retrieve for the specified <paramref name="pageIndex"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Used to propagate notification that operation should be canceled.
        /// </param>
        /// <returns>
        /// Returns a collection of <typeparamref name="TSource"/>.
        /// </returns>
        Task<IEnumerable<TSource>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class IncrementalLoadingCollection<TSource, IType>
        : ObservableCollection<IType>, ISupportIncrementalLoading where TSource : IIncrementalSource<IType>
    {
        /// <summary>
        /// Increment loading에 관련해서 초기 데이터를 가져온다.
        /// Gets a value indicating the source of incremental loading.
        /// </summary>
        protected TSource Source { get; set; }

        /// <summary>
        /// Increment Call로 한번에 가져올 양
        /// Gets a value indicating how many items that must be retrieved for each incremental call.
        /// </summary>
        protected int ItemsPerPage { get; }

        private bool _hasMoreItems;
        /// <summary>
        /// 추가로 가져올 아이템이 있는지 여부
        /// Gets a value indicating whether the collection contains more items to retrieve.
        /// 컬렉션에 검색 할 항목이 더 있는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                return _hasMoreItems;
            }
            private set
            {
                // 추가 값 설정 후 Collection에 알려주기
                if (value != _hasMoreItems)
                {
                    _hasMoreItems = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(HasMoreItems)));
                }
            }
        }

        private CancellationToken _cancellationToken;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="itemsPerPage"></param>
        public IncrementalLoadingCollection(int itemsPerPage = 20) : this(Activator.CreateInstance<TSource>(), itemsPerPage)
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="source"></param>
        /// <param name="itemsPerPage"></param>
        public IncrementalLoadingCollection(TSource source, int itemsPerPage = 20)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;
            ItemsPerPage = itemsPerPage;
            _hasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            => LoadMoreItemsAsync(count, new CancellationToken(false)).AsAsyncOperation();

        /// <summary>
        /// 아이템 추가로 가져와서 있으면 Collection에 추가
        /// </summary>
        /// <param name="count">가져오고 싶은 양</param>
        /// <param name="cancellationToken"></param>
        /// <returns>추가할, 추가된 아이템 갯수</returns>
        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            uint resultCount = 0;       // 추가된 갯수
            _cancellationToken = cancellationToken;

            if (!_cancellationToken.IsCancellationRequested)
            {
                IEnumerable<IType> data = null;
                data = await LoadDataAsync(_cancellationToken);
                resultCount = (uint)data.Count();

                // 데이터가 있고, 취소 요청이 아닐 때 데이터를 기존 Collection에 추가
                if (data != null && data.Any() && !_cancellationToken.IsCancellationRequested)
                {
                    foreach (var item in data)
                    {
                        Add(item);
                    }
                }
                else
                {
                    HasMoreItems = false;
                }
            }

            return new LoadMoreItemsResult() { Count = resultCount };
        }

        /// <summary>
        /// 추가적인 아이템 가져오기
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<IType>> LoadDataAsync(CancellationToken cancellationToken)
        {
            // 가져오고 싶은 Class(Source)에서 추가 항목 가져오기
            return await Source.GetPagedItemsAsync(0, ItemsPerPage, cancellationToken);
        }

        /// <summary>
        /// 초기화 및 다시 불러오기
        /// Clears the collection and triggers/forces a reload of the first page
        /// </summary>
        /// <returns>This method does not return a result</returns>
        public Task RefreshAsync()
        {
            var previousCount = Count;      // 현재 Collection 개수
            Clear();
            HasMoreItems = true;

            if (previousCount == 0)
            {
                // When the list was empty before clearing, the automatic reload isn't fired, so force a reload.
                // previousCount가 0이 아니면 실패했으므로 다시 실행해야함
                return LoadMoreItemsAsync(0).AsTask();
            }

            return Task.CompletedTask;
        }
    }
}
