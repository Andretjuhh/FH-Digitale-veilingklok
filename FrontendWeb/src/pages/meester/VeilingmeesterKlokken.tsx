import React, { useCallback, useMemo, useState } from 'react';
import Page from '../../components/nav/Page';
import { useRootContext } from '../../components/contexts/RootContext';
import { Column, DataTable, OnFetchHandlerParams } from '../../components/layout/Table';
import { KlokStatusBadge } from '../../components/elements/StatusBadge';
import { VeilingKlokOutputDto } from '../../declarations/dtos/output/VeilingKlokOutputDto';
import { useComponentStateReducer } from '../../hooks/useComponentStateReducer';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { getVeilingKlokken } from '../../controllers/server/veilingmeester';
import Button from '../../components/buttons/Button';
import CreateVeilingKlok from '../../components/sections/veiling-dashboard/CreateVeilingKlok';
import { VeilingKlokStatus } from '../../declarations/enums/VeilingKlokStatus';
import { getNormalizedVeilingKlokStatus } from '../../utils/standards';
import TableFilterDropdown from '../../components/buttons/TableFilterDropdown';

function VeilingmeesterVeilingBeheren() {
	const { t, account, languageCode, navigate } = useRootContext();

	const [paginatedVeilingenState, setPaginatedVeilingenState] = useComponentStateReducer();
	const [paginatedVeilingen, setPaginatedVeilingen] = useState<PaginatedOutputDto<VeilingKlokOutputDto>>();
	const [openCreateModal, setOpenCreateModal] = useState(false);
	const [statusFilter, setStatusFilter] = useState<VeilingKlokStatus | undefined>(undefined);

	const handleFetchVeilingen = useCallback(
		async (params: OnFetchHandlerParams) => {
			try {
				setPaginatedVeilingenState({ type: 'loading' });
				const response = await getVeilingKlokken(
					statusFilter,
					account?.region,
					undefined,
					undefined,
					undefined,
					undefined,
					undefined,
					undefined,
					undefined,
					params.page,
					params.pageSize,
				);
				if (response.data) setPaginatedVeilingen(response.data);
				setPaginatedVeilingenState({ type: 'succeed' });
			} catch (err) {
				console.error('Failed to fetch orders', err);
			}
		},
		[statusFilter],
	);

	const klokColumns: Column<VeilingKlokOutputDto>[] = useMemo(
		() => [
			{
				key: 'scheduledAt',
				label: t('scheduledDate'),
				sortable: true,
				render: (item) => (
					<span className="font-medium capitalize">
						{new Date(item.scheduledAt).toLocaleString(languageCode, {
							weekday: 'long',
							year: 'numeric',
							month: 'long',
							day: 'numeric',
							hour: '2-digit',
							minute: '2-digit',
						})}
					</span>
				),
			},
			{
				key: 'status',
				label: t('veilingklok_status'),
				sortable: true,
				render: (item) => <KlokStatusBadge status={item.status} />,
			},
			{
				key: 'totalProducts',
				label: t('totalProducts'),
				sortable: true,
				render: (item) => <span className="font-medium">{item.totalProducts}</span>,
			},
			{
				key: 'totalBids',
				label: t('totalBids'),
				sortable: true,
				render: (item) => <span className="font-medium">{item.currentBids}</span>,
			},
			{
				key: 'endedDate',
				label: t('endedDate'),
				sortable: true,
				render: (item) => (
					<span className="font-medium capitalize">
						{item.endedAt
							? new Date(item.endedAt).toLocaleDateString(languageCode, {
									weekday: 'long',
									year: 'numeric',
									month: 'long',
									day: 'numeric',
								})
							: '-- / -- / ----'}
					</span>
				),
			},
			{
				key: 'action',
				label: ' ',
				render: (item) => (
					<div className={'app-table-actions-row-btns'}>
						{getNormalizedVeilingKlokStatus(item.status) === VeilingKlokStatus.Scheduled && (
							<Button
								className={'app-table-actions-row-dlt-btn'}
								icon={'bi-trash-fill'}
								aria-label={t('aria_meester_delete_auction')}
								onClick={(e) => {
									e.stopPropagation();
									console.log(`Delete veilingklok ${item.id}`);
								}}
							/>
						)}
					</div>
				),
			},
		],
		[t, languageCode],
	);

	return (
		<Page enableHeader className="vm-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-products-page-ctn">
				<section className="page-title-section" aria-labelledby="meester-klokken-title meester-klokken-subtitle">
					<h1 id="meester-klokken-title">
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2 id="meester-klokken-subtitle">{t('manage_auction_txt')}</h2>
				</section>

				<DataTable<VeilingKlokOutputDto>
					key={`meester-veilingen-table-${statusFilter || 'all'}`} // Force remount on filter change
					isLazy
					enableSearch={false}
					loading={paginatedVeilingenState.type == 'loading'}
					data={paginatedVeilingen?.data || []}
					itemsPerPage={20}
					totalItems={paginatedVeilingen?.totalCount || 0}
					getItemKey={(item) => item.id}
					onFetchData={handleFetchVeilingen}
					onCellClick={(item) => navigate(`/veilingmeester/veilingen/${item.id}`)}
					title={t('recent_auctionclocks')}
					icon={<i className="bi bi-clock-fill"></i>}
					columns={klokColumns}
					filterGroups={
						<>
							<TableFilterDropdown
								menuItems={[
									{ id: undefined, label: t('all_statuses') },
									{ id: VeilingKlokStatus.Scheduled, label: t('scheduled') },
									{ id: VeilingKlokStatus.Started, label: t('started') },
									{ id: VeilingKlokStatus.Ended, label: t('ended') },
									{ id: VeilingKlokStatus.Paused, label: t('paused') },
									{ id: VeilingKlokStatus.Stopped, label: t('stopped') },
								]}
								selectedItemId={statusFilter}
								onSelectItem={(e) => setStatusFilter(e as any)}
							/>

							<Button
								icon={'bi-calendar-plus-fill'}
								label={t('schedule_veilingklok')}
								aria-label={t('aria_meester_schedule_auction')}
								onClick={() => setOpenCreateModal(true)}
							/>
						</>
					}
					emptyText={t('no_veilingen_planned')}
					aria-label={t('aria_meester_auction_list')}
				/>

				{openCreateModal && (
					<div className="modal-overlay" onClick={() => setOpenCreateModal(false)}>
						<CreateVeilingKlok
							onClose={() => setOpenCreateModal(false)}
							onSuccess={() => {
								window.location.reload();
							}}
						/>
					</div>
				)}
			</main>
		</Page>
	);
}

export default VeilingmeesterVeilingBeheren;
