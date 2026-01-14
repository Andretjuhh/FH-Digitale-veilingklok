import React, { useState } from 'react';
import { AccountListItemDTO } from '../../declarations/dtos/output/AccountListItemDTO';
import '../../styles/modal.css';

interface DeleteConfirmationModalProps {
	account: AccountListItemDTO;
	onConfirm: (hardDelete: boolean) => void;
	onCancel: () => void;
}

function DeleteConfirmationModal({ account, onConfirm, onCancel }: DeleteConfirmationModalProps) {
	const [deleteType, setDeleteType] = useState<'soft' | 'hard' | null>(null);

	const handleConfirm = () => {
		if (deleteType === null) return;
		onConfirm(deleteType === 'hard');
	};

	return (
		<div className="modal-overlay" onClick={onCancel}>
			<div className="modal-content" onClick={(e) => e.stopPropagation()}>
				<div className="modal-header">
					<h2>Delete Account</h2>
					<button className="modal-close" onClick={onCancel}>
						<i className="bi bi-x"></i>
					</button>
				</div>

				<div className="modal-body">
					<p className="modal-text">
						You are about to delete the account: <strong>{account.email}</strong>
					</p>
					<p className="modal-text">Please select the type of deletion:</p>

					<div className="delete-options">
						<label className={`delete-option ${deleteType === 'soft' ? 'selected' : ''}`}>
							<input
								type="radio"
								name="deleteType"
								value="soft"
								checked={deleteType === 'soft'}
								onChange={() => setDeleteType('soft')}
							/>
							<div className="option-content">
								<h3>Soft Delete</h3>
								<p>Mark the account as deleted. Data is preserved and can be restored later.</p>
							</div>
						</label>

						<label className={`delete-option ${deleteType === 'hard' ? 'selected' : ''}`}>
							<input
								type="radio"
								name="deleteType"
								value="hard"
								checked={deleteType === 'hard'}
								onChange={() => setDeleteType('hard')}
							/>
							<div className="option-content">
								<h3>Hard Delete (Permanent)</h3>
								<p className="warning-text">
									<i className="bi bi-exclamation-triangle-fill"></i>
									Permanently delete the account and all associated data. This action cannot be undone!
								</p>
							</div>
						</label>
					</div>
				</div>

				<div className="modal-footer">
					<button className="btn-cancel" onClick={onCancel}>
						Cancel
					</button>
					<button
						className="btn-confirm"
						onClick={handleConfirm}
						disabled={deleteType === null}
					>
						Confirm Delete
					</button>
				</div>
			</div>
		</div>
	);
}

export default DeleteConfirmationModal;
