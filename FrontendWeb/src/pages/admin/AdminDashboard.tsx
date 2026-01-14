// External imports
import React, { useEffect, useState } from 'react';
import { useRootContext } from '../../components/contexts/RootContext';
import Page from '../../components/nav/Page';
import { getAllAccounts, deleteAccount, reactivateAccount } from '../../controllers/server/account';
import { AccountListItemDTO } from '../../declarations/dtos/output/AccountListItemDTO';
import { isHttpError } from '../../declarations/types/HttpError';
import '../../styles/admin.css';
import DeleteConfirmationModal from '../../components/modals/DeleteConfirmationModal';
import ReactivateConfirmationModal from '../../components/modals/ReactivateConfirmationModal';

function AdminDashboard() {
	const { navigate } = useRootContext();
	const [accounts, setAccounts] = useState<AccountListItemDTO[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [selectedAccount, setSelectedAccount] = useState<AccountListItemDTO | null>(null);
	const [showDeleteModal, setShowDeleteModal] = useState(false);
	const [showReactivateModal, setShowReactivateModal] = useState(false);

	useEffect(() => {
		loadAccounts();
	}, []);

	const loadAccounts = async () => {
		try {
			setLoading(true);
			setError(null);
			const response = await getAllAccounts();
			setAccounts(response.data);
		} catch (err) {
			console.error('Failed to load accounts:', err);
			if (isHttpError(err)) {
				setError(err.message);
			} else {
				setError('Failed to load accounts');
			}
		} finally {
			setLoading(false);
		}
	};

	const handleDelete = (account: AccountListItemDTO) => {
		setSelectedAccount(account);
		setShowDeleteModal(true);
	};

	const handleReactivate = (account: AccountListItemDTO) => {
		setSelectedAccount(account);
		setShowReactivateModal(true);
	};

	const confirmDelete = async (hardDelete: boolean) => {
		if (!selectedAccount) return;

		try {
			setError(null);
			await deleteAccount(selectedAccount.id, hardDelete);
			setShowDeleteModal(false);
			setSelectedAccount(null);
			await loadAccounts();
		} catch (err) {
			console.error('Failed to delete account:', err);
			if (isHttpError(err)) {
				setError(err.message);
			} else {
				setError('Failed to delete account');
			}
		}
	};

	const confirmReactivate = async () => {
		if (!selectedAccount) return;

		try {
			setError(null);
			await reactivateAccount(selectedAccount.id);
			setShowReactivateModal(false);
			setSelectedAccount(null);
			await loadAccounts();
		} catch (err) {
			console.error('Failed to reactivate account:', err);
			if (isHttpError(err)) {
				setError(err.message);
			} else {
				setError('Failed to reactivate account');
			}
		}
	};

	const formatDate = (dateString: string) => {
		return new Date(dateString).toLocaleDateString('nl-NL', {
			year: 'numeric',
			month: 'long',
			day: 'numeric',
		});
	};

	return (
		<Page enableHeader={true} className="admin-dashboard">
			<div className="admin-container">
				<div className="admin-header">
					<h1>Admin Dashboard</h1>
					<p>Manage all user accounts</p>
				</div>

				{error && (
					<div className="admin-error">
						<i className="bi bi-exclamation-triangle-fill"></i>
						<span>{error}</span>
					</div>
				)}

				{loading ? (
					<div className="admin-loading">
						<i className="bi bi-arrow-repeat spinner"></i>
						<span>Loading accounts...</span>
					</div>
				) : (
					<div className="admin-table-container">
						<table className="admin-table">
							<thead>
								<tr>
									<th>Email</th>
									<th>Account Type</th>
									<th>Created At</th>
									<th>Status</th>
									<th>Actions</th>
								</tr>
							</thead>
							<tbody>
								{accounts.map((account) => (
									<tr key={account.id} className={account.isDeleted ? 'deleted-row' : ''}>
										<td>{account.email}</td>
										<td>
											<span className={`account-badge ${account.accountType.toLowerCase()}`}>
												{account.accountType}
											</span>
										</td>
										<td>{formatDate(account.createdAt)}</td>
										<td>
											{account.isDeleted ? (
												<span className="status-badge deleted">Soft Deleted</span>
											) : (
												<span className="status-badge active">Active</span>
											)}
										</td>
										<td>
											<div className="action-buttons">
												{account.isDeleted ? (
													<button
														className="btn-reactivate"
														onClick={() => handleReactivate(account)}
														title="Reactivate account"
													>
														<i className="bi bi-arrow-clockwise"></i>
														Reactivate
													</button>
												) : (
													<button
														className="btn-delete"
														onClick={() => handleDelete(account)}
														title="Delete account"
													>
														<i className="bi bi-trash"></i>
														Delete
													</button>
												)}
											</div>
										</td>
									</tr>
								))}
							</tbody>
						</table>
					</div>
				)}
			</div>

			{showDeleteModal && selectedAccount && (
				<DeleteConfirmationModal
					account={selectedAccount}
					onConfirm={confirmDelete}
					onCancel={() => {
						setShowDeleteModal(false);
						setSelectedAccount(null);
					}}
				/>
			)}

			{showReactivateModal && selectedAccount && (
				<ReactivateConfirmationModal
					account={selectedAccount}
					onConfirm={confirmReactivate}
					onCancel={() => {
						setShowReactivateModal(false);
						setSelectedAccount(null);
					}}
				/>
			)}
		</Page>
	);
}

export default AdminDashboard;
