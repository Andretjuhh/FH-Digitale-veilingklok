import React from 'react';
import { AccountListItemDTO } from '../../declarations/dtos/output/AccountListItemDTO';
import '../../styles/modal.css';

interface ReactivateConfirmationModalProps {
	account: AccountListItemDTO;
	onConfirm: () => void;
	onCancel: () => void;
}

function ReactivateConfirmationModal({ account, onConfirm, onCancel }: ReactivateConfirmationModalProps) {
	return (
		<div className="modal-overlay" onClick={onCancel}>
			<div className="modal-content" onClick={(e) => e.stopPropagation()}>
				<div className="modal-header">
					<h2>Reactivate Account</h2>
					<button className="modal-close" onClick={onCancel}>
						<i className="bi bi-x"></i>
					</button>
				</div>

				<div className="modal-body">
					<p className="modal-text">
						Are you sure you want to reactivate the account: <strong>{account.email}</strong>?
					</p>
					<p className="modal-text">
						This will restore the account to active status and allow the user to log in again.
					</p>
				</div>

				<div className="modal-footer">
					<button className="btn-cancel" onClick={onCancel}>
						Cancel
					</button>
					<button className="btn-confirm" onClick={onConfirm}>
						Confirm Reactivation
					</button>
				</div>
			</div>
		</div>
	);
}

export default ReactivateConfirmationModal;
