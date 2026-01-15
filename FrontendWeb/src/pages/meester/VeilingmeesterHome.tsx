import React, {useCallback, useMemo, useState} from 'react';
import {useRootContext} from "../../components/contexts/RootContext";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import {PaginatedOutputDto} from "../../declarations/dtos/output/PaginatedOutputDto";
import {VeilingKlokOutputDto} from "../../declarations/dtos/output/VeilingKlokOutputDto";
import {Column, DataTable, OnFetchHandlerParams} from "../../components/layout/Table";
import {getVeilingKlokken} from "../../controllers/server/veilingmeester";
import {KlokStatusBadge} from "../../components/elements/StatusBadge";
import Page from "../../components/nav/Page";
import CreateVeilingKlok from "../../components/sections/veiling-dashboard/CreateVeilingKlok";
import {KwekerProductStats} from "../../components/sections/kweker/KwekerStats";

function VeilingmeesterManageHome() {
	const {t, account, languageCode, navigate} = useRootContext();

	const [paginatedVeilingenState, setPaginatedVeilingenState] = useComponentStateReducer();
	const [paginatedVeilingen, setPaginatedVeilingen] = useState<PaginatedOutputDto<VeilingKlokOutputDto>>();
	const [openCreateModal, setOpenCreateModal] = useState(false);

	const handleFetchVeilingen = useCallback(async (params: OnFetchHandlerParams) => {
		try {
			setPaginatedVeilingenState({type: 'loading'});
			const response = await getVeilingKlokken(
				undefined,
				account?.region,
				undefined,
				undefined,
				undefined,
				undefined,
				undefined,
				undefined,
				undefined,
				params.page,
				params.pageSize
			);
			if (response.data) setPaginatedVeilingen(response.data);
			setPaginatedVeilingenState({type: 'succeed'});
		} catch (err) {
			console.error('Failed to fetch orders', err);
		}

	}, []);

	const klokColumns: Column<VeilingKlokOutputDto>[] = useMemo(
		() => [
			{
				key: 'scheduledAt',
				label: t('scheduledDate'),
				sortable: true,
				render: (item) => <span className="font-medium capitalize">{new Date(item.scheduledAt).toLocaleString(languageCode, {
					weekday: 'long',
					year: 'numeric',
					month: 'long',
					day: 'numeric',
					hour: '2-digit',
					minute: '2-digit'
				})}</span>
			},
			{
				key: 'status',
				label: t('veilingklok_status'),
				sortable: true,
				render: (item) => <KlokStatusBadge status={item.status}/>
			},
			{
				key: 'totalProducts',
				label: t('totalProducts'),
				sortable: true,
				render: (item) => <span className="font-medium">{item.totalProducts}</span>
			},
			{
				key: 'totalBids',
				label: t('totalBids'),
				sortable: true,
				render: (item) => <span className="font-medium">{item.currentBids}</span>
			},
			{
				key: 'endedDate',
				label: t('endedDate'),
				sortable: true,
				render: (item) => <span className="font-medium capitalize">
					{
						item.endedAt ?
							new Date(item.endedAt).toLocaleDateString(languageCode, {
								weekday: 'long',
								year: 'numeric',
								month: 'long',
								day: 'numeric'
							})
							: '-- / -- / ----'
					}
				</span>
			},
		],
		[t, languageCode]
	);

	return (
		<Page enableHeader className="vm-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-products-page-ctn">
				<section className="page-title-section">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2>
						{t('manage_auction_txt')}
					</h2>
				</section>

				<section className={'products-page-stats'}>
					<KwekerProductStats/>
				</section>

				<DataTable<VeilingKlokOutputDto>
					isLazy
					enableSearch={false}
					loading={paginatedVeilingenState.type == 'loading'}
					data={paginatedVeilingen?.data || []}
					itemsPerPage={20}
					totalItems={paginatedVeilingen?.totalCount || 0}
					getItemKey={item => item.id}
					onFetchData={handleFetchVeilingen}
					onCellClick={(item) => navigate(`/veilingmeester/veilingen-beheren/${item.id}`)}

					title={t('recent_auctionclocks')}
					icon={<i className="bi bi-clock-fill"></i>}
					columns={klokColumns}
					filterGroups={
						<>

						</>
					}
					emptyText={t('no_veilingen_planned')}
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