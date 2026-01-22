import React, { useCallback, useMemo, useState } from 'react';
import { useRootContext } from '../../components/contexts/RootContext';
import { useComponentStateReducer } from '../../hooks/useComponentStateReducer';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { VeilingKlokOutputDto } from '../../declarations/dtos/output/VeilingKlokOutputDto';
import { Column, DataTable, OnFetchHandlerParams } from '../../components/layout/Table';
import { getVeilingKlokken } from '../../controllers/server/veilingmeester';
import { KlokStatusBadge } from '../../components/elements/StatusBadge';
import Page from '../../components/nav/Page';
import CreateVeilingKlok from '../../components/sections/veiling-dashboard/CreateVeilingKlok';
import { MeesterStats } from '../../components/sections/meester/MeesterStats';
import TableFilterDropdown from '../../components/buttons/TableFilterDropdown';
import { VeilingKlokStatus } from '../../declarations/enums/VeilingKlokStatus';

function VeilingmeesterManageHome() {
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
		],
		[t, languageCode],
	);

	return (
		<Page enableHeader className="vm-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-products-page-ctn">
				<section className="page-title-section" aria-labelledby="meester-home-title meester-home-subtitle">
					<h1 id="meester-home-title">
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2 id="meester-home-subtitle">{t('manage_auction_txt')}</h2>
				</section>

				<MeesterStats aria-label={t('aria_meester_stats')} />

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
					onCellClick={(item) => navigate(`/veilingmeester/veilingen-beheren/${item.id}`)}
					title={t('recent_auctionclocks')}
					icon={<i className="bi bi-clock-fill"></i>}
					columns={klokColumns}
					emptyText={t('no_veilingen_planned')}
					aria-label={t('aria_meester_stats')}
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
						</>
					}
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

export default VeilingmeesterManageHome;
