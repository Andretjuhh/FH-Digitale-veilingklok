import { Transition } from 'framer-motion';

const Spring: Transition = { type: 'spring', duration: 0.4 /*type: 'spring', damping: 25, stiffness: 300*/ } as const;

export { Spring };
