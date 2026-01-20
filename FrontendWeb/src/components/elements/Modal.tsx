import React from 'react';

type ModalProps = {
	enabled: boolean;
	onClose: () => void;
	children?: React.ReactNode;
}

function Modal(props: ModalProps) {
	const {enabled, onClose, children} = props;

	if (!enabled) {
		return null;
	}

	return (
		<div className="modal-overlay" onClick={onClose}>
			{children && (
				<div className="modal-overlay-ctn" onClick={(e) => e.stopPropagation()}>
					{children}
				</div>
			)}
		</div>
	);
}

export default Modal;