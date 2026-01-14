import React from 'react';
import Spinner from "./Spinner";
import {State} from "../../hooks/useComponentStateReducer";
import Button from "../buttons/Button";
import {useRootContext} from "../contexts/RootContext";
import {LayoutGroup, motion} from "framer-motion";

function ComponentState(props: { state: State }) {
	const {state} = props;
	return (
		<div className="auth-state">
			{state.type === 'loading' && <Spinner/>}
			{state.type === 'succeed' && <i className="bi bi-check-circle-fill text-green-500"></i>}
			{state.type === 'error' && <i className="bi bi-x-circle-fill text-red-500"></i>}
			<p className="auth-state-text">{state.message}</p>
		</div>
	);
}

export function ComponentStateCard(props: { state: State, onClose?: () => void }) {
	const {state, onClose} = props;
	const {t} = useRootContext();

	return (
		<LayoutGroup>
			<motion.div layout className="modal-card component-state-card auto-height">
				<ComponentState state={state}/>
				{(onClose && state.type !== 'loading') &&
					<Button
						className="modal-card-close-btn"
						label={t('close')}
						onClick={onClose}
					/>
				}
			</motion.div>
		</LayoutGroup>
	);
}

export default ComponentState;