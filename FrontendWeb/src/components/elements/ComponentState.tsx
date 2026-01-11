import React from 'react';
import Spinner from "../ui/Spinner";
import {State} from "../../hooks/useComponentStateReducer";

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

export default ComponentState;