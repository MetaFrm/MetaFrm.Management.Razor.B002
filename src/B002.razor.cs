using MetaFrm.Database;
using MetaFrm.Extensions;
using MetaFrm.Management.Razor.Models;
using MetaFrm.Management.Razor.ViewModels;
using MetaFrm.Razor.DataGrid;
using MetaFrm.Razor.Group;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MetaFrm.Management.Razor
{
    /// <summary>
    /// B002
    /// </summary>
    public partial class B002
    {
        #region Variable
        internal B002ViewModel B002ViewModel { get; set; } = Factory.CreateViewModel<B002ViewModel>();

        internal DataGridControl<PermissionsModel>? DataGridControl;
        internal List<ColumnDefinitions>? ColumnDefinitions;

        internal PermissionsModel SelectItem = new();

        internal GroupWindowStatus GroupWindowStatus = GroupWindowStatus.Close;
        #endregion


        #region Init
        /// <summary>
        /// OnInitialized
        /// </summary>
        protected override void OnInitialized()
        {
            if (this.ColumnDefinitions == null)
            {
                this.ColumnDefinitions = new();
                this.ColumnDefinitions.AddRange(new ColumnDefinitions[] {
                    new ColumnDefinitions{ DataField = nameof(PermissionsModel.NAME), Caption = "Permissions", DataType = DbType.NVarChar, Class = "text-break", SortDirection = SortDirection.Ascending },
                    new ColumnDefinitions{ DataField = nameof(PermissionsModel.INACTIVE_DATE), Caption = "Inactive", DataType = DbType.DateTime, Class = "text-break", Alignment = Alignment.Center, SortDirection = SortDirection.Normal, Format = "yyyy-MM-dd HH:mm" }});
            }
        }

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.IsLogin())
                    this.Navigation?.NavigateTo("/", true);

                this.B002ViewModel = await this.GetSession<B002ViewModel>(nameof(this.B002ViewModel));

                this.Search();

                this.StateHasChanged();
            }
        }
        #endregion


        #region IO
        private void New()
        {
            if (this.SelectItem.RESPONSIBILITY_ID != null || this.GroupWindowStatus != GroupWindowStatus.Close)
                this.SelectItem = new();
            this.GroupWindowStatus = GroupWindowStatus.Maximize;
        }

        private void OnSearch()
        {
            if (this.DataGridControl != null)
                this.DataGridControl.CurrentPageNumber = 1;

            this.Search();
        }
        private void Search()
        {
            Response response;

            try
            {
                if (this.B002ViewModel.IsBusy) return;

                this.B002ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Search");
                serviceData["1"].AddParameter(nameof(this.B002ViewModel.SearchModel.SEARCH_TEXT), DbType.NVarChar, 4000, this.B002ViewModel.SearchModel.SEARCH_TEXT);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());
                serviceData["1"].AddParameter("PAGE_NO", DbType.Int, 3, this.DataGridControl != null ? this.DataGridControl.CurrentPageNumber : 1);
                serviceData["1"].AddParameter("PAGE_SIZE", DbType.Int, 3, this.DataGridControl != null && this.DataGridControl.PagingEnabled ? this.DataGridControl.PagingSize : int.MaxValue);

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.B002ViewModel.SelectResultModel.Clear();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0)
                    {
                        foreach (var datarow in response.DataSet.DataTables[0].DataRows)
                        {
                            this.B002ViewModel.SelectResultModel.Add(new PermissionsModel
                            {
                                RESPONSIBILITY_ID = datarow.Int(nameof(PermissionsModel.RESPONSIBILITY_ID)),
                                NAME = datarow.String(nameof(PermissionsModel.NAME)),
                                INACTIVE_DATE = datarow.DateTime(nameof(PermissionsModel.INACTIVE_DATE)),
                            });
                        }
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.B002ViewModel.IsBusy = false;
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                this.SetSession(nameof(B002ViewModel), this.B002ViewModel);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            }
        }

        private void Save()
        {
            Response? response;
            string? value;

            response = null;

            try
            {
                if (this.B002ViewModel.IsBusy)
                    return;

                if (this.SelectItem == null)
                    return;

                this.B002ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Save");
                serviceData["1"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3, "2", nameof(this.SelectItem.RESPONSIBILITY_ID), this.SelectItem.RESPONSIBILITY_ID);
                serviceData["1"].AddParameter(nameof(this.SelectItem.NAME), DbType.NVarChar, 50, this.SelectItem.NAME);
                serviceData["1"].AddParameter(nameof(this.SelectItem.INACTIVE_DATE), DbType.DateTime, 0, this.SelectItem.INACTIVE_DATE);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0 && this.SelectItem != null && this.SelectItem.RESPONSIBILITY_ID == null)
                    {
                        value = response.DataSet.DataTables[0].DataRows[0].String("Value");

                        if (value != null)
                            this.SelectItem.RESPONSIBILITY_ID = value.ToInt();
                    }

                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} registered successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.B002ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }

        private void Delete()
        {
            this.ModalShow($"Question", "Do you want to delete?", new() { { "Delete", Btn.Danger }, { "Cancel", Btn.Primary } }, EventCallback.Factory.Create<string>(this, DeleteAction));
        }
        private void DeleteAction(string action)
        {
            Response? response;

            response = null;

            try
            {
                if (action != "Delete") return;

                if (this.B002ViewModel.IsBusy) return;

                if (this.SelectItem == null) return;

                this.B002ViewModel.IsBusy = true;

                if (this.SelectItem.RESPONSIBILITY_ID == null || this.SelectItem.RESPONSIBILITY_ID <= 0)
                {
                    this.ToastShow("Warning", $"Please select a {this.GetAttribute("Title")} to delete.", Alert.ToastDuration.Long);
                    return;
                }

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Delete");
                serviceData["1"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3, this.SelectItem.RESPONSIBILITY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.New();
                    this.Close();
                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} deleted successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.B002ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }
        #endregion


        #region Event
        private void SearchKeydown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                this.OnSearch();
            }
        }

        private void RowModify(PermissionsModel item)
        {
            this.SelectItem = new()
            {
                RESPONSIBILITY_ID = item.RESPONSIBILITY_ID,
                NAME = item.NAME,
                INACTIVE_DATE = item.INACTIVE_DATE,
            };

            this.GroupWindowStatus = GroupWindowStatus.Maximize;
        }

        private void Copy()
        {
            if (this.SelectItem != null)
            {
                this.SelectItem.RESPONSIBILITY_ID = null;
            }
        }

        private void Close()
        {
            this.GroupWindowStatus = GroupWindowStatus.Close;
        }
        #endregion
    }
}