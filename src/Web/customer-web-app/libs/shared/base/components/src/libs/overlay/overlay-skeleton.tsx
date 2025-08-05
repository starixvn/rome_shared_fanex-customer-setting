import classNames from 'classnames';
import styles from './overlay-skeleton.module.scss';

type OverlaySize = 'small' | 'medium' | 'large' | 'fullscreen';

export const OverlaySkeleton: React.FC<{
    children: React.ReactNode;
    size?: OverlaySize;
}> = ({
    children,
    size = 'medium',
  }) => {
    
    const getOverlaySizeClass = () => {
      switch (size) {
        case 'small':
          return styles.overlaySmall;
        case 'large':
          return styles.overlayLarge;
        case 'fullscreen':
          return styles.overlayFullscreen;
        case 'medium':
        default:
          return styles.overlayMedium;
      }
    };

    return (
      <div className={classNames(styles.overlay, getOverlaySizeClass())}>
        {children}
      </div>
    );
  };