import {
  Component, OnInit, Inject,
  ViewChild, ContentChild, TemplateRef,
  ViewContainerRef, ElementRef, Input,
} from '@angular/core'
import { DataTableAccessor } from '../data-table-accessor'
import { ModalConfirmDeleteComponent, ConfirmDeleteModalContext } from '../modal-confirm-delete/modal-confirm-delete.component'
import { ToastsManager } from 'ng2-toastr'

const KEY_ENTER = 13

export class FilterContext {
  $implicit: any
  $search: Function
  $clearSearch: Function
  $updateParams: Function
  $filter: any
}

export class ToastContext {
  $implicit: any
  $response?: any
}

/**
 * @usage
 * <data-table #table [usePageLink]="false">
 *   <ng-template #deleteMessage let-item></ng-template>
 *   <ng-template #customFilter let-search let-updateParams="$updateParams"></ng-template>
 *   <ng-template #toastMessage let-response="$response" let-item>
 *     <span class="data-table-error-message">{{response.messages}}</span>
 *     <span class="data-table-success-message">{{item.name}} deleted successfully</span>
 *   </ng-template>
 *
 *   <table>
 *     <tbody>
 *       <tr *ngFor="let item of table.items"></tr>
 *     </tbody>
 *   </table>
 *   <ng-template #customFilter let-search let-filter="$filter" let-clear="$clearSearch">
 *     <input type="text" class="custom-search-box" (keyup)="search($event)">
 *   </ng-template>
 *   <ng-template #deleteMessage let-item>
 *     <p>Are you sure you want to delete {{item.name}}</p>
 *   </ng-template>
 * </data-table>
 */
@Component({
  selector: 'data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent implements OnInit {
  @Input() usePageLink = true
  @ViewChild(ModalConfirmDeleteComponent) modalConfirmDelete: ModalConfirmDeleteComponent
  @ContentChild('deleteMessage') deleteMessageTpl: TemplateRef<ConfirmDeleteModalContext>
  @ViewChild('customFilterVcr', { read: ViewContainerRef }) filterVcr: ViewContainerRef
  @ViewChild('defaultFilterTpl') defaultFilterTpl: TemplateRef<FilterContext>
  @ContentChild('customFilter') customFilterTpl: TemplateRef<FilterContext>

  @ViewChild('defaultToastTpl') defaultToastTpl: TemplateRef<ToastContext>
  @ContentChild('toastMessage') toastMessageTpl: TemplateRef<ToastContext>
  @ViewChild('toastMessageVcr', { read: ViewContainerRef }) toastVcr: ViewContainerRef
  @ViewChild('toastMessageContainer') private toastMessageContainer: ElementRef

  list: Array<any>
  paging: any = {}
  filter = {}
  private filterContext: FilterContext
  private toastContext: ToastContext

  constructor(
    @Inject(DataTableAccessor) private accessor: DataTableAccessor,
    private toastr: ToastsManager,
  ) { }

  ngOnInit() {
    this.accessor.data.subscribe(data => {
      this.list = data.items
      this.paging = data.paging
      this.filter = data.filter
      if (this.filterContext) {
        this.filterContext.$filter = data.filter
      }
    })
    this.modalConfirmDelete.renderTemplate(this.deleteMessageTpl)
    this.renderFilter()
    this.renderToast()
  }

  index(i) {
    const { page, quantity } = this.paging
    return (page - 1) * quantity + i + 1
  }

  delete(item) {
    this.modalConfirmDelete.open(item)
  }

  reload() {
    this.accessor.fetch()
  }

  updateParams(params?) {
    this.accessor.fetch(params)
  }

  search(event: KeyboardEvent) {
    if (event.keyCode !== KEY_ENTER) {
      return
    }

    const target = event.target as HTMLInputElement
    this.accessor.fetch({query: target.value})
  }

  clearSearch() {
    this.accessor.fetch({query: undefined})
  }

  onDeleteConfirmed(item) {
    const next = () => {
      this.toastContext.$implicit = item
      this.showToast('success')
      this.reload()
    }

    const error = reason => {
      this.toastContext.$implicit = item
      this.toastContext.$response = reason
      this.showToast('error')
    }
    return this.accessor.remove(item)
      .subscribe(next, error)
  }

  onPageChanged(page) {
    if (this.usePageLink) {
      return
    }

    this.updateParams({page})
  }

  private renderFilter() {
    const context: FilterContext = {
      $implicit: this.search.bind(this),
      $search: this.search.bind(this),
      $clearSearch: this.clearSearch.bind(this),
      $updateParams: this.updateParams.bind(this),
      $filter: this.filter
    }

    const tpl = this.customFilterTpl || this.defaultFilterTpl
    const view = this.filterVcr.createEmbeddedView(tpl, context)
    this.filterContext = view.context
  }

  private renderToast() {
    const tpl = this.toastMessageTpl || this.defaultToastTpl
    const view = this.toastVcr.createEmbeddedView(tpl, { $implicit: undefined })
    this.toastContext = view.context
  }

  private showToast(type: string) {
    const elm = this.toastMessageContainer.nativeElement as HTMLElement
    const content = elm.querySelector(`.data-table-message-${type}`)
    setTimeout(() => {
      this.toastr[type](content.innerHTML, type)
    })
  }
}
